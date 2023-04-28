using System.Text.Json;

namespace Application.Services.TokenService;

public class TokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly byte[] _tokenKey;
    private readonly double _refreshTokenTime;

    public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenKey = Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:TokenKey").Value!);
        _refreshTokenTime = Double.Parse(configuration.GetSection("AppSettings:RefreshTokenTime").Value!);
    }

    public string CreateAuthToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(_tokenKey);

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_refreshTokenTime),
                signingCredentials: creds
        );

        token.Payload["iat"] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public (RefreshToken, DateTime) CreateRefreshToken(Guid userId)
    {
        return (
            new RefreshToken
            {
                Id = userId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            },
            DateTime.UtcNow.AddMinutes(_refreshTokenTime)
        );
    }

    // Block user refresh token by expiring it
    public void BlockRefreshToken(ref User user)
    {
        user.RefreshTokenExpires_UTC = DateTime.UtcNow.AddYears(-1);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddYears(-1)
        };

        _httpContextAccessor.HttpContext!.Response.Cookies.Delete("refreshToken", cookieOptions);
    }

    public async Task<string?> GetAuthToken() => await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");

    public RefreshToken? GetRefreshToken()
    {
        var cookieRefreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(cookieRefreshToken))
            return null;

        RefreshToken? receivedRefreshToken = null;

        try
        {
            receivedRefreshToken = JsonSerializer.Deserialize<RefreshToken>(cookieRefreshToken)!;
        }
        catch
        {
            return null;
        }

        return receivedRefreshToken;
    }

    public void SetRefreshTokenCookie(RefreshToken refreshToken, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = expires
        };

        if(_httpContextAccessor.HttpContext is not null)
            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", JsonSerializer.Serialize(refreshToken), cookieOptions);
    }
}
