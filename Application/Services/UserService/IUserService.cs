namespace Application.Services.UserService;

public interface IUserService
{
    Task<(HttpStatusCode, ServiceResponse<User_DTO>)> GetMyProfile();
    Task<(HttpStatusCode, ServiceResponse<User_DTO>)> LoadUserById(Guid request);
    Task<(HttpStatusCode, ServiceResponse<User_DTO>)> LoadUserByUsername(string request);
    Task<(HttpStatusCode, ServiceResponse<User_DTO>)> UpdateUser(User_DTO request);
    Task<(HttpStatusCode, ServiceResponse<bool>)> DeleteUserById(Guid request);
    Task<(HttpStatusCode, ServiceResponse<List<User_DTO>>)> LoadManyUsers(LoadManyUsers_DTO request);
}

