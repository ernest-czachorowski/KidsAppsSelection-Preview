namespace Application.Services.GetUserService;

public class ReceivedTokenService : IReceivedTokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReceivedTokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid IdFromToken() 
        => (Guid.TryParse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid guid)) ? guid : Guid.Empty;

    public string? EmailFromToken() 
        => _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);

    public string? UsernameFromToken() 
        => _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name);

    public string? RoleFromToken() 
        => _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role);
}
