namespace Presentation.Razor.Default.Components;

public class SelectorBase_T<TItem> : TwoWayBinding_T<TItem>
{
    [Parameter]
    public string Label { get; set; } = string.Empty;
}

