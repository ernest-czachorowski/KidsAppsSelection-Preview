namespace Presentation.Services;

public interface IUserService
{
    Task<ServiceResponse<User_DTO>> GetMyProfile();
    Task<ServiceResponse<User_DTO>> LoadUserById(Guid request);
    Task<ServiceResponse<User_DTO>> LoadUserByUsername(string request);
    Task<ServiceResponse<User_DTO>> UpdateUser(User_DTO request);
    Task<ServiceResponse<List<User_DTO>>> LoadManyUsers(LoadManyUsers_DTO request);
    Task<ServiceResponse<bool?>> DeleteUserById(Guid request);
}