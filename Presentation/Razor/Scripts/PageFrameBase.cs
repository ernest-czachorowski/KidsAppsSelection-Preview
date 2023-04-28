namespace Presentation.Razor.Scripts;

public class PageFrameBase<TMainObject> : MudComponentBase where TMainObject : new()
{
    [Inject]
    protected ISnackbar SnackbarService { get; set; }

    protected TMainObject mainObject = new TMainObject();
    protected string message = string.Empty;
    protected PageStatus pageStatus = PageStatus.Init;

    protected virtual void ChangePageStatus(PageStatus status = PageStatus.DataLoaded)
    {
        pageStatus = status;
    }

    protected virtual void ChangePageStatus(PageStatus status = PageStatus.DataLoaded, string msg = "", Severity severity = Severity.Info)
    {
        message = msg;

        if (!string.IsNullOrEmpty(message))
            SnackbarService.Add(message, severity);

        pageStatus = status;
    }
}

