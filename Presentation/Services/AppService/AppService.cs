namespace Presentation.Services;

public class AppService : IAppService
{
    private readonly HttpClient _httpClient;
    private readonly IResponseManagerService _responseManagerService;

    public AppService(HttpClient httpClient, IResponseManagerService responseManagerService)
	{
        _httpClient = httpClient;
        _responseManagerService = responseManagerService;
    }

    public async Task<ServiceResponse<App_DTO>> AddApp(AddApp_DTO request)
    {
        var response = await _httpClient.TryPutAsync("API/App/AddApp", request);
        return await _responseManagerService.Parse<App_DTO>(response);
    }

    public async Task<ServiceResponse<bool?>> DeleteAppById(Guid request)
    {
        var response = await _httpClient.TryDeleteAsync($"API/App/Admin/DeleteAppById/{request}");
        return await _responseManagerService.Parse<bool?>(response);
    }

    public async Task<ServiceResponse<List<App_DTO>>> LoadAllApps()
    {
        var response = await _httpClient.TryGetAsync("API/App/Admin/LoadAllApps");
        return await _responseManagerService.Parse<List<App_DTO>>(response);
    }

    public async Task<ServiceResponse<App_DTO>> LoadAppById(Guid request)
    {
        var response = await _httpClient.TryGetAsync($"API/App/Admin/LoadAppById/{request}");
        return await _responseManagerService.Parse<App_DTO>(response);
    }

    public async Task<ServiceResponse<List<App_DTO>>> LoadManyApps(LoadManyApps_DTO request)
    {
        var response = await _httpClient.TryPostAsync("API/App/LoadManyApps", request);
        return await _responseManagerService.Parse<List<App_DTO>>(response);
    }

    public async Task<ServiceResponse<App_DTO>> UpdateApp(App_DTO request)
    {
        var response = await _httpClient.TryPatchAsync($"API/App/Admin/UpdateApp", request);
        return await _responseManagerService.Parse<App_DTO>(response);
    }
}

