namespace Presentation.Services;

public interface IAuthService
{
    Task<ServiceResponse<string?>> Register(UserRegister_DTO request);
    Task<ServiceResponse<string?>> Login(UserLogin_DTO request);
    Task<ServiceResponse<bool?>> ChangePassword(UserChangePassword_DTO request);
    Task<ServiceResponse<bool?>> DeleteAccount(UserLogin_DTO request);
    Task<ServiceResponse<bool?>> Logout();
}
