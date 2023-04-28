namespace Presentation.Razor.Default.Pages;

public class ProfileBase : PageFrameMain<User_DTO>
{
    [Inject]
    protected IUserService UserService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        ChangePageStatus(PageStatus.Loading);

        var response = await UserService.GetMyProfile();

        if (response.Success)
        {
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
            mainObject = response.Data;
        }
        else
        {
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
        }
    }
}