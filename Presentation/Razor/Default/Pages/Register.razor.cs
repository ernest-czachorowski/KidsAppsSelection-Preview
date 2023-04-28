namespace Presentation.Razor.Default.Pages;

public class RegisterBase : PageFrameMain<UserRegister_DTO>
{
    protected async Task RegisterHandler()
    {
        if (!IsFormValid())
            return;

        ChangePageStatus(PageStatus.Loading);

        await TryLogout();

        var response = await AuthService.Register(mainObject);

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

