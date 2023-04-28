namespace Presentation.Razor.Admin.Pages;

public class Admin_EditUserBase : PageFrameBase<User_DTO>
{
    [Inject]
    public IDialogService DialogService { get; set; }

    [Inject]
    public IUserService UserService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter, EditorRequired]
    public Guid guid { get; set; }

    private User_DTO _mainObjectBackup = null;

    protected override async Task OnInitializedAsync()
    {
        ChangePageStatus(PageStatus.Loading);

        var response = await UserService.LoadUserById(guid);

        if (response.Success)
        {
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
            mainObject = response.Data;
            _mainObjectBackup = mainObject.DeepCopyByJson<User_DTO>();
        }
        else
        {
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
        }

    }

    protected async Task SubmitFormHandler(EditContext context)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Do you really want to submit form?" },
            { "ButtonText", "Submit" },
            { "Color", Color.Success }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Edit user", parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        ChangePageStatus(PageStatus.Loading);

        var response = await UserService.UpdateUser(mainObject);

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

    protected async Task DeleteUserHandler(Guid id)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Do you really want to delete this user? This process cannot be undone." },
            { "ButtonText", "Delete" },
            { "Color", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete user", parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        ChangePageStatus(PageStatus.Loading);

        var response = await UserService.DeleteUserById(id);

        if (response.Success)
        {
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
            NavigationManager.NavigateTo("/Admin/ManageUsers");
        }
        else
        {
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
        }

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

        mainObject = _mainObjectBackup.DeepCopyByJson<User_DTO>();
        SnackbarService.Add("Data restrored from backup.", Severity.Success);

        StateHasChanged();
    }
}

