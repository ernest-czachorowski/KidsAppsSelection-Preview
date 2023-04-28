namespace Presentation.Razor.Default.Pages;

public partial class AppsBase : PageFrameBase<List<App_DTO>>
{
    [Inject]
    protected IAppService AppService { get; set; }

    protected int page = 1;
    protected int itemsToLoad = GLOBAL.ITEMS_TO_LOAD;
    protected int carouselSize = 0;

    protected AppPlatform appPlatform = AppPlatform.Any;
    protected AppStatus appStatus = AppStatus.Visible;
    protected string searchedText = string.Empty;


    protected override async Task OnInitializedAsync()
    {
        await LoadMoreItems();
    }

    protected async Task OnAppPlatformChangedHandler(AppPlatform platform)
    {
        if (platform.Equals(appPlatform))
            return;

        ChangePageStatus(PageStatus.Init);

        mainObject = new();
        page = 1;
        appPlatform = platform;

        await LoadMoreItems();
    }

    protected async Task OnAppStatusChangedHandler(AppStatus status)
    {
        if (status.Equals(appStatus))
            return;

        ChangePageStatus(PageStatus.Init);

        mainObject = new();
        page = 1;
        appStatus = status;

        await LoadMoreItems();
    }

    protected async Task SearchTextHandler(string text)
    {
        if (pageStatus == PageStatus.Loading)
            return;

        ChangePageStatus(PageStatus.Init);

        mainObject = new();
        page = 1;
        searchedText = (string.IsNullOrEmpty(text)) ? "" : text;

        await LoadMoreItems();
    }

    protected async Task LoadMoreItems()
    {
        if (pageStatus == PageStatus.Loading || pageStatus == PageStatus.NoMoreData)
            return;

        ChangePageStatus(PageStatus.Loading);

        LoadManyApps_DTO request = new LoadManyApps_DTO
        {
            Start = (page - 1) * itemsToLoad,
            ItemsToLoad = itemsToLoad,
            Status = appStatus,
            Platform = appPlatform,
            SearchedText = searchedText
        };

        var response = await AppService.LoadManyApps(request);

        if (response.Success && (response.Data is null || !response.Data.Any()))
        {
            ChangePageStatus(PageStatus.NoMoreData, response.Message, Severity.Info);
            return;
        }
        else if (response.Success && response.Data.Count < itemsToLoad)
        {
            mainObject.AddRange(response.Data);
            ChangePageStatus(PageStatus.NoMoreData, response.Message, Severity.Info);
            return;
        }
        else if (!response.Success || response.Data is null)
        {
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
            return;
        }

        mainObject.AddRange(response.Data);

        page++;
        //ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Info);
        ChangePageStatus(PageStatus.DataLoaded);
    }
}

