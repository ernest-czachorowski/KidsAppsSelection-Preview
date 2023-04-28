namespace Application.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly IReceivedTokenService _receivedTokenService;
    private readonly ITokenService _tokenService;
    private readonly ITokenBlockingService _tokenBlockingService;

    public AuthService(DataContext context, IReceivedTokenService receivedTokenService, ITokenService tokenService, ITokenBlockingService tokenBlockingService)
    {
        _context = context;
        _receivedTokenService = receivedTokenService;
        _tokenService = tokenService;
        _tokenBlockingService = tokenBlockingService;
    }

    public async Task<(HttpStatusCode, ServiceResponse<string>)> Login(UserLogin_DTO request)
    {
        var dbResult = await _context.LoadUserByEmailAsync(request.Email);

        if (dbResult is null)
            return HttpResponse<string>.NotFound($"The requested user with email: {request.Email} could not be found in the database.");
        else if (!VerifyPasswordHash(request.Password, dbResult.PasswordHash, dbResult.PasswordSalt))
            return HttpResponse<string>.Unauthorized($"Wrong password.");

        (var refreshToken, var expiresAt) = _tokenService.CreateRefreshToken(dbResult.Id);

        _context.Entry(dbResult).State = EntityState.Modified;

        dbResult.RefreshToken = refreshToken.Token;
        dbResult.RefreshTokenExpires_UTC = expiresAt;

        await _context.SaveChangesAsync();

        _tokenService.SetRefreshTokenCookie(refreshToken, expiresAt);

        return HttpResponse<string>.OK(_tokenService.CreateAuthToken(dbResult), $"Token created.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<string>)> Register(UserRegister_DTO request, UserRole userRole = UserRole.User)
    {
        if (await _context.IsEmialAsync(request.Email))
            return HttpResponse<string>.Confilct($"A user with the email: {request.Email} already exists in the database.");

        if (await _context.IsUsernameAsync(request.Username))
            return HttpResponse<string>.Confilct($"A user with the username: {request.Username} already exists in the database.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            CreateDate_UTC = DateTime.UtcNow,
            UpdateDate_UTC = DateTime.UtcNow,
            Role = userRole
        };

        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        (var refreshToken, var expiresAt) = _tokenService.CreateRefreshToken(user.Id);
        user.RefreshToken = refreshToken.Token;
        user.RefreshTokenExpires_UTC = expiresAt;

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        _tokenService.SetRefreshTokenCookie(refreshToken, expiresAt);

        return HttpResponse<string>.Created(_tokenService.CreateAuthToken(user), "Registration successful.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<string>)> RefreshToken(bool request)
    {
        RefreshToken? receivedRefreshToken = _tokenService.GetRefreshToken();

        if(receivedRefreshToken is null)
            return HttpResponse<string>.InternalServerError("Cannot read refresh token.");

        var dbResult = await _context.LoadUserByIdAsync(receivedRefreshToken.Id);

        if (dbResult == null)
            return HttpResponse<string>.InternalServerError("Requested user not found.");

        if (dbResult.RefreshTokenExpires_UTC < DateTime.UtcNow)
            return HttpResponse<string>.Unauthorized("Refresh token already expired.");

        // If someone is trying to use wrong refresh token
        if (!dbResult.RefreshToken.Equals(receivedRefreshToken.Token))
        {
            await _tokenBlockingService.BlockAuthTokens(dbResult.Id);

            _tokenService.BlockRefreshToken(ref dbResult);
            _context.Entry(dbResult).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return HttpResponse<string>.Unauthorized("Refresh token is not correct.");
        }

        (var newRefreshToken, var expiresAt) = _tokenService.CreateRefreshToken(dbResult.Id);
        dbResult.RefreshToken = newRefreshToken.Token;
        dbResult.RefreshTokenExpires_UTC = expiresAt;

        _context.Entry(dbResult).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        _tokenService.SetRefreshTokenCookie(newRefreshToken, expiresAt);

        return HttpResponse<string>.Created(_tokenService.CreateAuthToken(dbResult), "Token refreshed.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<bool>)> ChangePassword(UserChangePassword_DTO request)
    {
        var user = await _context.LoadUserByIdAsync(_receivedTokenService.IdFromToken());

        if (user is null)
            return HttpResponse<bool>.NotFound($"The requested user could not be found in the database.");
        else if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            return HttpResponse<bool>.Unauthorized($"Wrong password.");

        _context.Entry(user).State = EntityState.Modified;

        CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        _tokenService.BlockRefreshToken(ref user);

        await _context.SaveChangesAsync();

        await _tokenBlockingService.BlockAuthTokens(user.Id);

        return HttpResponse<bool>.OK(true, $"Your password has been changed.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<bool>)> DeleteAccount(UserLogin_DTO request)
    {
        var user = await _context.LoadUserByIdAsync(_receivedTokenService.IdFromToken());

        if (user is null)
            return HttpResponse<bool>.NotFound($"The requested user could not be found in the database.");
        else if (user.Email != request.Email)
            return HttpResponse<bool>.Unauthorized($"Wrong email.");
        else if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            return HttpResponse<bool>.Unauthorized($"Wrong password.");

        _tokenService.BlockRefreshToken(ref user);
        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        await _tokenBlockingService.BlockAuthTokens(user.Id);

        return HttpResponse<bool>.OK(true, $"Your account has been deleted.");
    }

    public async Task<(HttpStatusCode, ServiceResponse<bool>)> Logout()
    {
        var user = await _context.LoadUserByIdAsync(_receivedTokenService.IdFromToken());

        if (user is null)
            return HttpResponse<bool>.NotFound($"The requested user could not be found in the database.");

        _tokenService.BlockRefreshToken(ref user);

        await _tokenBlockingService.BlockAuthTokens(user.Id);

        return HttpResponse<bool>.OK(true, $"Token blocked.");
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}
