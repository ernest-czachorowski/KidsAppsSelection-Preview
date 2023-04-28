namespace Application.Services.ServicesExtensionMethods;

public static class AppExtensions
{
    private static readonly Func<DataContext, string, string, Task<bool>> IsAppWithUrlOrTittle_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, string url, string title) => context.Apps
            .AsNoTracking()
            .Any(a => a.Url.Equals(url) || a.Title.Equals(title))
    );

    public static async Task<bool> IsAppWithUrlOrTittleAsync(this DataContext context, string url, string title)
        => await IsAppWithUrlOrTittle_CompiledQuery(context, url, title);




    private static readonly Func<DataContext, IAsyncEnumerable<App>> LoadAllApps_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context) => context.Apps
            .AsNoTracking()
            .Include(a => a.AddedBy)
            .OrderBy(a => a.Title)
            .AsQueryable()
    );

    public static async Task<List<App>> LoadAllAppsAsync(this DataContext context)
    {
        List<App> apps = new();

        await foreach (var item in LoadAllApps_CompiledQuery(context))
            apps.Add(item);

        return apps;
    }




    private static readonly Func<DataContext, Guid, Task<App?>> LoadAppById_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, Guid appId) => context.Apps
            .AsNoTracking()
            .FirstOrDefault(a => a.Id == appId)
    );

    public static async Task<App?> LoadAppByIdAsync(this DataContext context, Guid id)
        => await LoadAppById_CompiledQuery(context, id);




    private static readonly Func<DataContext, Guid, Task<App?>> LoadAppByIdWithIncludes_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, Guid appId) => context.Apps
            .AsNoTracking()
            .Include(a => a.AddedBy)
            .FirstOrDefault(a => a.Id == appId)
    );

    public static async Task<App?> LoadAppByIdWithIncludesAsync(this DataContext context, Guid id)
        => await LoadAppByIdWithIncludes_CompiledQuery(context, id);




    private static readonly Func<DataContext, string, AppStatus, AppPlatform, int, int, IAsyncEnumerable<App>> LoadManyApps_CompiledQuery = EF.CompileAsyncQuery(
        (DataContext context, string searchedText, AppStatus status, AppPlatform platform, int start, int itemsToLoad) => context.Apps
            .AsNoTracking()
            .Include(a => a.AddedBy)
            .Where(a => string.IsNullOrEmpty(searchedText) || a.Title.ToLower().StartsWith(searchedText.ToLower()))
            .Where(a => status == AppStatus.Any || a.Status == status)
            .Where(a => platform == AppPlatform.Any || a.Platform == platform)
            .OrderBy(a => a.DailyRandom)
            .Skip(start)
            .Take(itemsToLoad)
            .AsQueryable()
    );

    public static async Task<List<App>> LoadManyAppsAsync(this DataContext context, string searchedText, AppStatus status, AppPlatform platform, int start, int itemsToLoad)
    {
        List<App> apps = new();

        await foreach (var item in LoadManyApps_CompiledQuery(context, searchedText, status, platform, start, itemsToLoad))
            apps.Add(item);

        return apps;
    }
}
