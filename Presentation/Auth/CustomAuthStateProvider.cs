namespace Presentation.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpClient _httpClient;

    public CustomAuthStateProvider(ILocalStorageService localStorageService, HttpClient httpClient)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string authToken = await _localStorageService.GetItemAsStringAsync("authToken");

        var identity = new ClaimsIdentity();
        _httpClient.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(authToken) && !IsTokenExpired(authToken.Replace("\"", "")))
        {
            try
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));
            }
            catch
            {
                await _localStorageService.RemoveItemAsync("authToken");
                identity = new ClaimsIdentity();
            }
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));

        return claims;
    }

    private bool IsTokenExpired(string token)
    {
        JwtSecurityToken decodedToken = null;

        try
        {
            decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch
        {
            Console.WriteLine($"Error: Cannot read token. {token}");
            return true;
        }

        if (decodedToken is null || decodedToken.Payload is null || decodedToken.Payload.Exp is null)
        {
            Console.WriteLine("Error: Token is not valid.");
            return true;
        }

        if (!long.TryParse(decodedToken.Payload.Exp.ToString(), out long exp))
        {
            Console.WriteLine($"Error: Token has incorrect 'Iat' payload: {decodedToken.Payload.Exp}.");
            return true;
        }

        var timeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

        if (exp < timeNow)
        {
            Console.WriteLine("Error: Token expired.");
            return true;
        }

        return false;
    }
}
