namespace Presentation.Razor.Default.Pages;

public class DeleteAccountBase : PageFrameMain<UserLogin_DTO>
{
    [Inject]
    public IDialogService DialogService { get; set; }

    protected async Task DeleteHandler()
    {
        if (!IsFormValid())
            return;

        var parameters = new DialogParameters
        {
            { "ContentText", "Do you really want to delete your account? This process cannot be undone." },
            { "ButtonText", "Delete" },
            { "Color", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete account", parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        ChangePageStatus(PageStatus.Loading);

        var response = await AuthService.DeleteAccount(mainObject);

        if (response.Success)
        {
            await TryRemoveToken();
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
            NavigationManager.NavigateTo("/");
        }
        else
        {
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
        }
    }
}

