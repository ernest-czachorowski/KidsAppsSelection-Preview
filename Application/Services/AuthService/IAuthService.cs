namespace Application.Services.AuthService;

public interface IAuthService
{
    Task<(HttpStatusCode, ServiceResponse<string>)> Register(UserRegister_DTO request, UserRole userRole = UserRole.User);
    Task<(HttpStatusCode, ServiceResponse<string>)> RefreshToken(bool request);
    Task<(HttpStatusCode, ServiceResponse<string>)> Login(UserLogin_DTO request);
    Task<(HttpStatusCode, ServiceResponse<bool>)> ChangePassword(UserChangePassword_DTO request);
    Task<(HttpStatusCode, ServiceResponse<bool>)> DeleteAccount(UserLogin_DTO request);
    Task<(HttpStatusCode, ServiceResponse<bool>)> Logout();
}

