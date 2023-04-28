namespace Presentation.Razor.Scripts;

public class PageFrameMain<TMainObject> : PageFrameBase<TMainObject> where TMainObject : new()
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected IAuthService AuthService { get; set; }

    [Inject]
    protected ITokenService TokenService { get; set; }

    protected EditContext editContext;

    protected override void OnInitialized()
    {
        editContext = new(mainObject);
    }

    public async Task TrySetToken(string token)
    {
        if (await TokenService.IsUserAuthenticated())
            return;

        await TokenService.SetToken(token);
        SnackbarService.Add("Token set. You are logged in.", Severity.Success);
    }

    public async Task TryRemoveToken()
    {
        if (!await TokenService.IsUserAuthenticated())
            return;

        await TokenService.RemoveToken();
        SnackbarService.Add("Token removed. You have been logged out.", Severity.Success);
    }

    public async Task TryLogout()
    {
        if (!await TokenService.IsUserAuthenticated())
            return;

        ServiceResponse<bool?> response = await AuthService.Logout();

        if (response.Success)
            SnackbarService.Add(response.Message, Severity.Success);
        else
            SnackbarService.Add(response.Message, Severity.Error);

        await TryRemoveToken();

        // Give the server a little bit of time between blocking the old token and generating a new one.
        await Task.Delay(2000);
    }

    protected virtual bool IsFormValid()
    {
        if (!editContext.Validate())
        {
            SnackbarService.Add("Invalid input. Please fix errors before submitting the form.", Severity.Error);
            return false;
        }

        return true;
    }
}

