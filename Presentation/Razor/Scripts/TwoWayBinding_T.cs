namespace Presentation.Razor.Scripts;

public class TwoWayBinding_T<TItem> : MudComponentBase
{
    [Parameter, EditorRequired]
    public EventCallback<TItem> BindingValueChanged { get; set; }

    private TItem _bindingValue = default;

    [Parameter, EditorRequired]
    public TItem BindingValue
    {
        get => _bindingValue;
        set
        {
            if (_bindingValue != null && _bindingValue.Equals(value))
                return;

            _bindingValue = value;
        }
    }

    public async Task ChangeBindingValue(TItem value)
    {
        await BindingValueChanged.InvokeAsync(value);
    }
}

