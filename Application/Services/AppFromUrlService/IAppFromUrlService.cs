namespace Application.Services.AppFromUrlService
{
    public interface IAppFromUrlService
    {
        Task<App> GooglePlayStore(string url);
        string GetAppTitleFromGooglePlayStore(in string html);
        string GetIconUrlFromGooglePlayStore(in string html);
        List<string> GetImagesUrlsFromGooglePlayStore(in string html);
    }
}
