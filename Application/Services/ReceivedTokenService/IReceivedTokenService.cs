using Microsoft.EntityFrameworkCore;

namespace Application.Services.GetUserService;

public interface IReceivedTokenService
{
    public Guid IdFromToken();
    public string? EmailFromToken();
    public string? UsernameFromToken();
    public string? RoleFromToken();
}
