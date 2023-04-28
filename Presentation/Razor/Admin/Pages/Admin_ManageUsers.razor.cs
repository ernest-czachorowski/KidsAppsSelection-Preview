namespace Presentation.Razor.Admin.Pages;

public partial class Admin_ManageUsersBase : PageFrameBase<List<User_DTO>>
{
    [Inject]
    protected IUserService UserService { get; set; }

    protected int page = 1;
    protected int itemsToLoad = GLOBAL.ITEMS_TO_LOAD;

    protected string searchedText = string.Empty;

    protected UserRole userRole = UserRole.User;

    protected override async Task OnInitializedAsync()
    {
        await LoadMoreItems();
    }

    protected async Task OnRoleChangedHandler(UserRole role)
    {
        if (role.Equals(userRole))
            return;

        ChangePageStatus(PageStatus.Init);

        mainObject = new();
        page = 1;
        userRole = role;

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
        if (pageStatus == PageStatus.Loading || pageStatus == PageStatus.NoMoreData || pageStatus == PageStatus.Error)
            return;

        ChangePageStatus(PageStatus.Loading);

        LoadManyUsers_DTO request = new LoadManyUsers_DTO
        {
            Start = (page - 1) * itemsToLoad,
            ItemsToLoad = itemsToLoad,
            Role = userRole,
            SearchedText = searchedText
        };

        var response = await UserService.LoadManyUsers(request);

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

