namespace Presentation.Razor.Admin.Pages;

public class Admin_EditAppBase : PageFrameBase<App_DTO>
{
    [Inject]
    public IDialogService DialogService { get; set; }

    [Inject]
    public IAppService AppService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter, EditorRequired]
    public Guid guid { get; set; }

    private App_DTO _appBackup = null;
    private App_DTO _appBackupForImageUrl = null;
    protected int carouselSize = 0;

    protected override async Task OnInitializedAsync()
    {
        ChangePageStatus(PageStatus.Loading);

        var response = await AppService.LoadAppById(guid);

        if (response.Success)
        {
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
            mainObject = response.Data;
            _appBackup = mainObject.DeepCopyByJson<App_DTO>();
            _appBackupForImageUrl = mainObject.DeepCopyByJson<App_DTO>();
        }
        else
        {
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
        }
    }

    protected async Task SubmitFormHandler(EditContext editContext)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Do you really want to submit form?" },
            { "ButtonText", "Submit" },
            { "Color", Color.Success }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Edit app", parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        ChangePageStatus(PageStatus.Loading);

        var response = await AppService.UpdateApp(mainObject);

        if (response.Success)
        {
            mainObject = response.Data;
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
        }
        else
        {
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
        }

        StateHasChanged();
    }

    protected void ReplaceImages(string[] newImages)
    {
        mainObject.Images = newImages;

        StateHasChanged();
    }

    protected async Task RestoreFromBackupHandler()
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Do you really want to restore local copy of data?" },
            { "ButtonText", "Restore" },
            { "Color", Color.Secondary }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Restore data", parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        mainObject = _appBackup.DeepCopyByJson<App_DTO>();
        _appBackupForImageUrl = _appBackup.DeepCopyByJson<App_DTO>();
        SnackbarService.Add("Data restrored from backup.", Severity.Success);

        StateHasChanged();
    }

    protected async Task DeleteAppHandler()
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Do you really want to delete this app? This process cannot be undone." },
            { "ButtonText", "Delete" },
            { "Color", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete app", parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        ChangePageStatus(PageStatus.Loading);

        var response = await AppService.DeleteAppById(guid);

        if (response.Success)
        {
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
            NavigationManager.NavigateTo("/Admin/ManageApps");
        }
        else
        {
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
        }

        StateHasChanged();
    }
}

