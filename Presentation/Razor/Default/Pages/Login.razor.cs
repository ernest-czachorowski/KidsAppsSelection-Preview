namespace Presentation.Razor.Default.Pages;

public class LoginBase : PageFrameMain<UserLogin_DTO>
{
    protected async Task LoginHandler()
    {
        if (!IsFormValid())
            return;

        ChangePageStatus(PageStatus.Loading);

        await TryLogout();

        var response = await AuthService.Login(mainObject);

        if (response.Success)
        {
            await TrySetToken(response.Data);
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
            NavigationManager.NavigateTo("/");
        }
        else
        {
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
        }
    }
}

