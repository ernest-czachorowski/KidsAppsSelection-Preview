namespace Presentation.Razor.Default.Components;

partial class WarningIfValueChanged
{
    [Parameter, EditorRequired]
    public EventCallback RestoreValueHandler { get; set; }
}