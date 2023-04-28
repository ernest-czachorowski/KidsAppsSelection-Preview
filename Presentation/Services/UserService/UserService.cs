namespace Presentation.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly IResponseManagerService _responseManagerService;

    public UserService(HttpClient httpClient, IResponseManagerService responseManagerService)
    {
        _httpClient = httpClient;
        _responseManagerService = responseManagerService;
    }

    public async Task<ServiceResponse<bool?>> DeleteUserById(Guid request)
    {
        var response = await _httpClient.TryDeleteAsync($"API/User/Admin/DeleteUserById/{request}");
        return await _responseManagerService.Parse<bool?>(response);
    }

    public async Task<ServiceResponse<User_DTO>> GetMyProfile()
    {
        var response = await _httpClient.TryGetAsync("API/User/GetMyProfile");
        return await _responseManagerService.Parse<User_DTO>(response);
    }

    public async Task<ServiceResponse<List<User_DTO>>> LoadManyUsers(LoadManyUsers_DTO request)
    {
        var response = await _httpClient.TryPostAsync("API/User/Admin/LoadManyUsers", request);
        return await _responseManagerService.Parse<List<User_DTO>>(response);
    }

    public async Task<ServiceResponse<User_DTO>> LoadUserById(Guid request)
    {
        var response = await _httpClient.TryGetAsync($"API/User/Admin/LoadUserById/{request}");
        return await _responseManagerService.Parse<User_DTO>(response);
    }

    public async Task<ServiceResponse<User_DTO>> LoadUserByUsername(string request)
    {
        var response = await _httpClient.TryGetAsync($"API/User/Admin/LoadUserByUsername/{request}");
        return await _responseManagerService.Parse<User_DTO>(response);
    }

    public async Task<ServiceResponse<User_DTO>> UpdateUser(User_DTO request)
    {
        var response = await _httpClient.TryPatchAsync($"API/User/Admin/UpdateUser", request);
        return await _responseManagerService.Parse<User_DTO>(response);
    }
}