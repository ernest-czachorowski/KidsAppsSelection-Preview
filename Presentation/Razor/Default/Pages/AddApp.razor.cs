namespace Presentation.Razor.Default.Pages;

public class AddAppBase : PageFrameMain<AddApp_DTO>
{
    [Inject]
    protected IAppService AppService { get; set; }

    protected async Task AddAppHandler()
    {
        if (!IsFormValid())
            return;

        ChangePageStatus(PageStatus.Loading);

        var response = await AppService.AddApp(mainObject);

        if (response.Success)
            ChangePageStatus(PageStatus.DataLoaded, response.Message, Severity.Success);
        else
            ChangePageStatus(PageStatus.Error, response.Message, Severity.Error);
    }
}

