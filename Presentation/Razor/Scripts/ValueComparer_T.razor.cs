namespace Presentation.Razor.Default.Components;

public class ValueComparer_T<TItem> : MudComponentBase
{
    [Inject]
    public ISnackbar SnackbarService { get; set; }

    private TItem _currentValue;

    [Parameter, EditorRequired]
    public TItem CurrentValue
    {
        get => _currentValue;
        set
        {
            if (_currentValue != null && _currentValue.Equals(value))
                return;

            _currentValue = value;
            _autosaveNeeded = true;
        }
    }

    [Parameter, EditorRequired]
    public EventCallback<TItem> OnValueSaved { get; set; }

    private TItem _backupValue;

    [Parameter]
    public TItem BackupValue
    {
        get => _backupValue;
        set => _backupValue = value.DeepCopyByJson<TItem>();
    }

    protected override void OnInitialized()
    {
        BackupValue = CurrentValue;
    }

    private bool _autosaveNeeded = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_autosaveNeeded)
        {
            await OnValueSaved.InvokeAsync(CurrentValue);
            _autosaveNeeded = false;
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected async Task RestoreValueHandler()
    {
        CurrentValue = BackupValue.DeepCopyByJson<TItem>();
        await OnValueSaved.InvokeAsync(CurrentValue);
        SnackbarService.Add("Value restored!", Severity.Info);
    }
}