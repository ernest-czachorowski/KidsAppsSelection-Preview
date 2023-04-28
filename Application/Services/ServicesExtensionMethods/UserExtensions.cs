using System.Net.NetworkInformation;
using Azure.Core;

namespace Application.Services.ServicesExtensionMethods;

public static class UserExtensions
{
    private static readonly Func<DataContext, string, Task<bool>> IsEmial_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, string request) => context.Users
            .AsNoTracking()
            .Any(u => u.Email.ToLower().Equals(request.ToLower()))
    );

    public static async Task<bool> IsEmialAsync(this DataContext context, string email)
        => await IsEmial_CompiledQuery(context, email);




    private static readonly Func<DataContext, string, Task<bool>> IsUsername_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, string request) => context.Users
            .AsNoTracking()
            .Any(u => u.Username.ToLower().Equals(request.ToLower()))
    );

    public static async Task<bool> IsUsernameAsync(this DataContext context, string username)
        => await IsUsername_CompiledQuery(context, username);




    private static readonly Func<DataContext, string, Task<User?>> LoadUserByEmial_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, string email) => context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Email.ToLower().Equals(email.ToLower()))
    );

    public static async Task<User?> LoadUserByEmailAsync(this DataContext context, string email)
        => await LoadUserByEmial_CompiledQuery(context, email);




    private static readonly Func<DataContext, Guid, Task<User?>> LoadUserById_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, Guid userId) => context.Users
            .AsNoTracking()
            .FirstOrDefault(a => a.Id == userId)
    );

    public static async Task<User?> LoadUserByIdAsync(this DataContext context, Guid id)
        => await LoadUserById_CompiledQuery(context, id);




    private static readonly Func<DataContext, Guid, Task<User?>> LoadUserByIdWithIncludes_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, Guid userId) => context.Users
            .AsNoTracking()
            .Include(u => u.Apps.OrderBy(a => a.Title))
            .FirstOrDefault(a => a.Id == userId)
    );

    public static async Task<User?> LoadUserByIdWithIncludesAsync(this DataContext context, Guid id)
        => await LoadUserByIdWithIncludes_CompiledQuery(context, id);




    private static readonly Func<DataContext, string, Task<User?>> LoadUserByUsername_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, string request) => context.Users
            .AsNoTracking()
            .Include(u => u.Apps.OrderBy(i => i.Title))
            .FirstOrDefault(u => u.Username == request)
    );

    public static async Task<User?> LoadUserByUsernameWithIncludesAsync(this DataContext context, string username)
        => await LoadUserByUsername_CompiledQuery(context, username);




    private static readonly Func<DataContext, string, UserRole, int, int, IAsyncEnumerable<User>> LoadManyUsers_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, string searchedText, UserRole role, int start, int itemsToLoad) => context.Users
            .AsNoTracking()
            .Include(u => u.Apps.OrderBy(i => i.Title))
            .Where(a => string.IsNullOrEmpty(searchedText) || a.Username.ToLower().StartsWith(searchedText.ToLower()))
            .Where(u => u.Role.Equals(role))
            .OrderBy(u => u.Username)
            .Skip(start)
            .Take(itemsToLoad)
            .AsQueryable()
    );

    public static async Task<List<User>> LoadManyUsersAsync(this DataContext context, string searchedText, UserRole role, int start, int itemsToLoad)
    {
        List<User> users = new();

        await foreach (var item in LoadManyUsers_CompiledQuery(context, searchedText, role, start, itemsToLoad))
            users.Add(item);

        return users;
    }
}
