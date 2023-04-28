namespace Application.Controllers;

[ApiController]
[Route("API/[controller]")]
public class AppController : ControllerBase
{
    private readonly IAppService _service;

    public AppController(IAppService service)
    {
        _service = service;
    }

    [HttpPut("AddApp"), Authorize]
    public async Task<ActionResult<ServiceResponse<App_DTO>>> AddApp(AddApp_DTO request)
    {
        var response = await _service.AddApp(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpDelete("Admin/DeleteAppById/{request}"), Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteAppById(Guid request)
    {
        var response = await _service.DeleteAppById(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpGet("Admin/LoadAllApps"), Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ServiceResponse<List<App_DTO>>>> LoadAllApps()
    {
        var response = await _service.LoadAllApps();
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpGet("Admin/LoadAppById/{request}"), Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ServiceResponse<App_DTO>>> LoadAppById(Guid request)
    {
        var response = await _service.LoadAppById(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpPost("LoadManyApps")]
    public async Task<ActionResult<ServiceResponse<List<App_DTO>>>> LoadManyApps(LoadManyApps_DTO request)
    {
        var response = await _service.LoadManyApps(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpPatch("Admin/UpdateApp"), Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ServiceResponse<App_DTO>>> UpdateApp(App_DTO request)
    {
        var response = await _service.UpdateApp(request);
        return StatusCode((int)response.Item1, response.Item2);
    }
}

