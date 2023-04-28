namespace Presentation.Razor.Default.Components;

public class AppPreviewContainerBase : MudComponentBase
{
    [Parameter, EditorRequired]
    public App_DTO App { get; set; }

    [Parameter, EditorRequired]
    public int GallerySize { get; set; }

    protected string goToStoreIcon = Icons.Material.Filled.QuestionMark;

    protected override void OnInitialized()
    {
        goToStoreIcon = App.Platform switch
        {
            AppPlatform.Android => Icons.Material.Filled.Android,
            AppPlatform.iOS => Icons.Custom.Brands.Apple,
            AppPlatform.Windows => Icons.Custom.Brands.Microsoft,
            AppPlatform.Mac => Icons.Material.Filled.LaptopMac,
            AppPlatform.Any => Icons.Material.Filled.QuestionMark
        };
    }
}

