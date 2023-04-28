namespace Presentation.Razor.Default.Components;

public class CarouselBase : MudComponentBase
{
    [Parameter, EditorRequired]
    public App_DTO App { get; set; }

    [Parameter, EditorRequired]
    public int GallerySize
    {
        get
        {
            return gallerySize;
        }
        set
        {
            gallerySize = value;
            carouselVisibility = (gallerySize > 0) ? "" : "display: none;";
            carouselSize = (gallerySize > 0) ? $"min-height: {gallerySize}rem;" : "min-height: 0px;";
            StateHasChanged();
        }
    }

    protected int gallerySize = 0;
    protected string carouselVisibility = "display: none;";
    protected string carouselSize = "min-height: 0px;";
}