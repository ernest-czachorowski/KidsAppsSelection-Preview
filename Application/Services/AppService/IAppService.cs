namespace Application.Services.AppService;

public interface IAppService
{
    Task<(HttpStatusCode, ServiceResponse<List<App_DTO>>)> LoadAllApps();
    Task<(HttpStatusCode, ServiceResponse<App_DTO>)> LoadAppById(Guid request);
    Task<(HttpStatusCode, ServiceResponse<App_DTO>)> UpdateApp(App_DTO request);
    Task<(HttpStatusCode, ServiceResponse<bool>)> DeleteAppById(Guid request);
    Task<(HttpStatusCode, ServiceResponse<App_DTO>)> AddApp(AddApp_DTO request);
    Task<(HttpStatusCode, ServiceResponse<App_DTO>)> ValidateApp(App request);
    Task<(HttpStatusCode, ServiceResponse<List<App_DTO>>)> LoadManyApps(LoadManyApps_DTO request);
}