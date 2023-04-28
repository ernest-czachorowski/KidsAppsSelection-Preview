namespace Presentation.Services;

public interface ITokenService
{
    public Task<bool> IsUserAuthenticated();
    public Task SetToken(string token);
    public Task RefreshToken();
    public Task RemoveToken();
}

