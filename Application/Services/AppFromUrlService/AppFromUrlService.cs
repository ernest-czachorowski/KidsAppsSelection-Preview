namespace Application.Services.AppFromUrlService;

public class AppFromUrlService : IAppFromUrlService
{
    private static HttpClient httpClient = new HttpClient();
    private static int _maxImages = 8;

    private readonly ILogger<AppFromUrlService> _logger;

    public AppFromUrlService(ILogger<AppFromUrlService> logger)
    {
        _logger = logger;
    }

    public async Task<App> GooglePlayStore(string url)
    {
        App app = new App();

        if (!url.StartsWith("https://play.google.com/store/apps/details?id="))
            return app;

        url = url.Split("&")[0];
        url = url + "&hl=en_US&gl=US";

        app.Url = url;
        app.Platform = AppPlatform.Android;

        string html = String.Empty;

        try
        {
            _logger.LogInformation($"The website data is being downloaded from: {url}");

            Stream data = await httpClient.GetStreamAsync(url);

            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            _logger.LogInformation($"The website data has been successfully downloaded from: {url}");
        }
        catch (IOException e)
        {
            _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            return app;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            return app;
        }

        if (string.IsNullOrEmpty(html))
        {
            _logger.LogError($"The website data downloaded from {url} is null or empty.");
            return app;
        }

        var taskTitle = Task.Run(() => GetAppTitleFromGooglePlayStore(in html));
        var taskIcon = Task.Run(() => GetIconUrlFromGooglePlayStore(in html));
        var taskImages = Task.Run(() => GetImagesUrlsFromGooglePlayStore(in html));

        try
        {
            Task.WaitAll(taskTitle, taskIcon, taskImages);
        }
        catch (AggregateException ae)
        {
            foreach (var e in ae.InnerExceptions)
            {
                _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            }
            return app;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            return app;
        }

        app.Title = HttpUtility.HtmlDecode(taskTitle.Result);
        app.Icon = taskIcon.Result;
        app.Images = taskImages.Result.ToArray();

        return app;
    }

    public string GetAppTitleFromGooglePlayStore(in string html)
    {
        string title = string.Empty;

        try
        {
            Match m = Regex.Match(html, "itemprop=\"name\"><span>(.*?)</span></h1>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            title = m.Groups[1].Value;
        }
        catch (RegexParseException e)
        {
            _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            return string.Empty;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            return string.Empty;
        }

        if(title == string.Empty)
        {
            _logger.LogError($"The regular expression was unable to find the app title.");
            return string.Empty;
        }

        return title;
    }

    public string GetIconUrlFromGooglePlayStore(in string html)
    {
        List<string> icons = new List<string>();

        try
        {
            foreach (Match m in Regex.Matches(html, "<img.+?src=[\"'](.+?)[\"'].+?alt=\"Icon image\".+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled))
            {
                icons.Add(m.Groups[1].Value);

                if (icons.Count > 1)
                    break;
            }
        }
        catch (RegexParseException e)
        {
            _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            return string.Empty;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            return string.Empty;
        }

        if (icons.Count != 2)
        {
            _logger.LogError($"The regular expression was unable to find the app icon");
            return string.Empty;
        }

        return icons[1];
    }

    public List<string> GetImagesUrlsFromGooglePlayStore(in string html)
    {
        List<string> images = new List<string>();

        try
        {
            foreach (Match m in Regex.Matches(html, "<img.+?src=[\"'](.+?)[\"'].+?alt=\"Screenshot image\".+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled))
            {
                images.Add(m.Groups[1].Value);

                if (images.Count > _maxImages)
                    break;
            }
        }
        catch (RegexParseException e)
        {
            _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            return images;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            return images;
        }

        if (!images.Any())
        {
            _logger.LogError($"The regular expression was unable to find the app images");
            return images;
        }

        // Removing some user icon 
        images.RemoveAt(0);

        return images;
    }

}
