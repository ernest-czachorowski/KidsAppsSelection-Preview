using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Razor.Default.Pages;

public class DebugBase : PageFrameMain<UserLogin_DTO>
{
    [Inject]
    protected ILocalStorageService LocalStorageService { get; set; }

    protected string _currentToken = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await TryReadToken();
    }
    private async Task LoginHandler()
    {
        ChangePageStatus(PageStatus.Loading);

        await TryLogout();

        var response = await AuthService.Login(mainObject);

        if (response.Success)
        {
            await TrySetToken(response.Data);
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
        }
        else
        {
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
        }

        await TryReadToken();
    }

    protected async Task LoginAsHandler(string login)
    {
        mainObject = new UserLogin_DTO() { Email = login, Password = "Pa$$w0rd" };
        await LoginHandler();
    }

    protected async Task LogoutButKeepTokenHandler()
    {
        ChangePageStatus(PageStatus.Loading);

        if (!await TokenService.IsUserAuthenticated())
        {
            ChangePageStatus(PageStatus.DataLoaded);
            return;
        }

        var response = await AuthService.Logout();

        if (response.Success)
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
        else
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);

        await TryReadToken();
    }

    protected async Task LogoutDontNavigate()
    {
        ChangePageStatus(PageStatus.Loading);

        if (!await TokenService.IsUserAuthenticated())
        {
            ChangePageStatus(PageStatus.DataLoaded);
            return;
        }

        var response = await AuthService.Logout();

        await TokenService.RemoveToken();

        if (response.Success)
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
        else
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);

        await TryReadToken();
    }

    protected async Task SetTokenHandler()
    {
        ChangePageStatus(PageStatus.Loading);

        await TokenService.SetToken(_currentToken);
        StateHasChanged();

        ChangePageStatus(PageStatus.DataLoaded, "Token sent to Authentication State Provider.", Severity.Info);
    }

    protected async Task RemoveTokenHandler()
    {
        await TokenService.RemoveToken();
        await TryReadToken();
    }

    protected async Task TryReadToken()
    {
        try
        {
            var token = await LocalStorageService.GetItemAsStringAsync("authToken");
            _currentToken = token.Replace("\"", "");
        }
        catch (Exception e)
        {
            await Console.Out.WriteLineAsync($"Cannot read token: {e}");
            _currentToken = string.Empty;
        }
        finally
        {
            StateHasChanged();
        }
    }
}
