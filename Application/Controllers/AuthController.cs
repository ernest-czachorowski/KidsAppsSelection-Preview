namespace Application.Controllers;

[ApiController]
[Route("API/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService authService)
    {
        _service = authService;
    }

    [HttpPut("Register")]
    public async Task<ActionResult<ServiceResponse<string>>> Register(UserRegister_DTO request)
    {
        var response = await _service.Register(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpPost("RefreshToken")]
    public async Task<ActionResult<ServiceResponse<RefreshToken>>> RefreshToken(bool request)
    {
        var response = await _service.RefreshToken(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpPost("Login")]
    public async Task<ActionResult<ServiceResponse<string>>> Login(UserLogin_DTO request)
    {
        var response = await _service.Login(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpPatch("ChangePassword"), Authorize]
    public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword(UserChangePassword_DTO request)
    {
        var response = await _service.ChangePassword(request);
        return StatusCode((int)response.Item1, response.Item2);
    }

    [HttpPost("Delete"), Authorize]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteAccount(UserLogin_DTO request)
    {
        var result = await _service.DeleteAccount(request);
        return StatusCode((int)result.Item1, result.Item2);
    }

    [HttpPost("Logout"), Authorize]
    public async Task<ActionResult<ServiceResponse<bool>>> Logout(bool request)
    {
        var response = await _service.Logout();
        return StatusCode((int)response.Item1, response.Item2);
    }
}

