namespace Presentation.Razor.Default.Pages;

public class ChangePasswordBase : PageFrameMain<UserChangePassword_DTO>
{
    protected async Task ChangePasswordHandler()
    {
        if (!IsFormValid())
            return;

        ChangePageStatus(PageStatus.Loading);

        var response = await AuthService.ChangePassword(mainObject);

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

