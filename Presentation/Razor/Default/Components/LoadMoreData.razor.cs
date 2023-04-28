namespace Presentation.Razor.Default.Components;

public partial class LoadMoreData
{
    [Parameter, EditorRequired]
    public PageStatus PageStatus { get; set; }

    [Parameter, EditorRequired]
    public EventCallback LoadMoreItems { get; set; }
}

