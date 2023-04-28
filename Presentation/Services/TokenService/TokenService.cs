namespace Presentation.Services;

public class TokenService : ITokenService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly RefreshTokenState _refreshTokenState;
    private const int _refreshTokenGap_s = 15;

    public TokenService(HttpClient httpClient, RefreshTokenState refreshTokenState, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        _refreshTokenState = refreshTokenState;
    }

    public async Task<bool> IsUserAuthenticated()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();

        if (authState?.User?.Identity is not null && authState.User.Identity.IsAuthenticated)
            return true;

        return false;
    }

    public async Task SetToken(string token)
    {
        await _localStorage.SetItemAsync("authToken", token);

        if(await IsUserAuthenticated())
        {
            var exp = ReadAuthTokenExpDate(token);

            if (exp is null)
                return;

            var refreshRequired_UTC = DateTimeOffset.FromUnixTimeSeconds(exp.Value).UtcDateTime - DateTime.UtcNow.AddSeconds(_refreshTokenGap_s);

            await Console.Out.WriteLineAsync($"Next token refresh required in: {(int)refreshRequired_UTC.TotalSeconds} s.");

            SetTimeout setTimeout = new SetTimeout(EventCallback.Factory.Create(this, RefreshToken), (int)refreshRequired_UTC.TotalMilliseconds);
            _refreshTokenState.SetRefreshTokenTimeout(setTimeout);
        }
    }

    public async Task RefreshToken()
    {
        ServiceResponse<string>? result = null;

        try
        {
            var response = await _httpClient.TryPostAsync($"API/Auth/RefreshToken", true);
            result = await response!.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }
        catch(Exception e)
        {
            await Console.Out.WriteLineAsync($"Error: Cannot refresh token: {e}");
            await RemoveToken();
            return;
        }

        if(result is null)
        {
            await Console.Out.WriteLineAsync($"Error: Cannot refresh token: {nameof(result)} is null");
            await RemoveToken();
            return;
        }

        if (result.Success && result.Data is not null)
            await SetToken(result.Data);
    }

    public async Task RemoveToken()
    {
        _refreshTokenState.UnsetRefreshTokenTimeout();
        await _localStorage.RemoveItemAsync("authToken");
        await _authStateProvider.GetAuthenticationStateAsync();
    }

    private long? ReadAuthTokenExpDate(string token)
    {
        JwtSecurityToken? decodedToken = null;

        try
        {
            decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch
        {
            Console.WriteLine("Error: JwtSecurityTokenHandler cannot read token.");
            return null;
        }

        if (decodedToken is null || decodedToken.Payload is null || decodedToken.Payload.Exp is null)
        {
            Console.WriteLine("Error: Token is not valid. It does not have correct payload.");
            return null;
        }

        if (!long.TryParse(decodedToken.Payload.Exp.ToString(), out long exp))
        {
            Console.WriteLine($"Error: Cannot parse token with incorrect 'Iat' payload: {decodedToken.Payload.Exp}.");
            return null;
        }

        return exp;
    }
}
