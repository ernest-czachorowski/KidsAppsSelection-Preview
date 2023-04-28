namespace Presentation.Razor.Default.Pages;

public class LogoutBase : PageFrameMain<Object>
{
    protected override async Task OnInitializedAsync()
    {
        await Logout();
        NavigationManager.NavigateTo("/");
    }

    protected async Task Logout()
    {
        ChangePageStatus(PageStatus.Loading);

        if (!await TokenService.IsUserAuthenticated())
        {
            ChangePageStatus(PageStatus.DataLoaded, "You are already logged out, skipping." , Severity.Warning);
            return;
        }

        var response = await AuthService.Logout();

        if (response.Success)
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
        else
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);

        await TryRemoveToken();

        // Give the server a little bit of time between blocking the old token and generating a new one.
        await Task.Delay(2000);
    }
}