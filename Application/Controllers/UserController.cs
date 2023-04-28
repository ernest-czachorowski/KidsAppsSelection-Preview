namespace Application.Controllers;

[ApiController]
[Route("API/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpDelete("Admin/DeleteUserById/{request}"), Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteUserById(Guid request)
    {
        var response = await _service.DeleteUserById(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpGet("GetMyProfile"), Authorize]
    public async Task<ActionResult<ServiceResponse<User_DTO>>> GetMyProfile()
    {
        var response = await _service.GetMyProfile();
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpPost("Admin/LoadManyUsers"), Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ServiceResponse<List<User_DTO>>>> LoadManyUsers(LoadManyUsers_DTO request)
    {
        var response = await _service.LoadManyUsers(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpGet("Admin/LoadUserById/{request}"), Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ServiceResponse<User_DTO>>> LoadUserById(Guid request)
    {
        var response = await _service.LoadUserById(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpGet("Admin/LoadUserByUsername/{request}"), Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ServiceResponse<User_DTO>>> LoadUserByUsername(string request)
    {
        var response = await _service.LoadUserByUsername(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpPatch("Admin/UpdateUser"), Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ServiceResponse<User_DTO>>> UpdateUser(User_DTO request)
    {
        var response = await _service.UpdateUser(request);
        return StatusCode((int)response.Item1, response.Item2);
    }
}

