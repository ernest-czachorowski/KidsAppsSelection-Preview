namespace Application.Services.TokenService;

public interface ITokenService
{
    string CreateAuthToken(User user);
    (RefreshToken, DateTime) CreateRefreshToken(Guid userId);
    void BlockRefreshToken(ref User user);
    void SetRefreshTokenCookie(RefreshToken refreshToken, DateTime expire);
    Task<string?> GetAuthToken();
    RefreshToken? GetRefreshToken();
}

