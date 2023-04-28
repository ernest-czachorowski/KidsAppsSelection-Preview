namespace Presentation.Razor.Default.Pages;

public class IndexBase : PageFrameMain<UserLogin_DTO>
{
    private async Task LoginHandler()
    {
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
    protected async Task LoginAsHandler(string login)
    {
        mainObject = new UserLogin_DTO() { Email = login, Password = "Pa$$w0rd" };
        await LoginHandler();
    }
}
