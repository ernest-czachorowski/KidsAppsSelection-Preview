namespace Presentation.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IResponseManagerService _responseManagerService;

    public AuthService(HttpClient httpClient, IResponseManagerService responseManagerService)
    {
        _httpClient = httpClient;
        _responseManagerService = responseManagerService;
    }

    public async Task<ServiceResponse<string?>> Register(UserRegister_DTO request)
    {
        var response = await _httpClient.TryPutAsync("API/Auth/Register", request);
        return await _responseManagerService.Parse<string?>(response);
    }

    public async Task<ServiceResponse<string?>> Login(UserLogin_DTO request)
    {
        var response = await _httpClient.TryPostAsync("API/Auth/Login", request);
        return await _responseManagerService.Parse<string?>(response);
    }

    public async Task<ServiceResponse<bool?>> Logout()
    {
        var response = await _httpClient.TryPostAsync("API/Auth/Logout", true);
        return await _responseManagerService.Parse<bool?>(response);
    }

    public async Task<ServiceResponse<bool?>> ChangePassword(UserChangePassword_DTO request)
    {
        var response = await _httpClient.TryPatchAsync("API/Auth/ChangePassword", request);
        return await _responseManagerService.Parse<bool?>(response); 
    }

    public async Task<ServiceResponse<bool?>> DeleteAccount(UserLogin_DTO request)
    {
        var response = await _httpClient.TryPostAsync($"API/Auth/Delete", request);
        return await _responseManagerService.Parse<bool?>(response);
    }
}
