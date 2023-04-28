namespace Presentation.Services;

public interface IAppService
{
    Task<ServiceResponse<App_DTO>> LoadAppById(Guid request);
    Task<ServiceResponse<App_DTO>> AddApp(AddApp_DTO request);
    Task<ServiceResponse<List<App_DTO>>> LoadManyApps(LoadManyApps_DTO request);
    Task<ServiceResponse<List<App_DTO>>> LoadAllApps();
    Task<ServiceResponse<App_DTO>> UpdateApp(App_DTO request);
    Task<ServiceResponse<bool?>> DeleteAppById(Guid request);
}

