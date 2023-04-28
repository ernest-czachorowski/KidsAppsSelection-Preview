namespace Presentation.Razor.Admin.Components;

public partial class Admin_ImageEditionBars
{
    [Inject]
    public IDialogService DialogService { get; set; }

    [Parameter, EditorRequired]
    public string[] Images { get; set; }

    [Parameter, EditorRequired]
    public EventCallback<string[]> OnImagesChanged { get; set; }

    private string[] _backups { get; set; }

    protected override void OnInitialized()
    {
        _backups = Images.DeepCopyByJson<string[]>();
    }

    public async Task ChangeIndexHandler(int currentIndex, int newIndex)
    {
        if (newIndex < 0 || newIndex >= Images.Length)
            return;

        string currentImage = Images[currentIndex];
        string newImage = Images[newIndex];

        string backupCurrentImage = _backups[currentIndex];
        string backupNewImage = _backups[newIndex];

        Images[newIndex] = currentImage;
        Images[currentIndex] = newImage;

        _backups[newIndex] = backupCurrentImage;
        _backups[currentIndex] = backupNewImage;

        await OnImagesChanged.InvokeAsync(Images);
    }

    public async Task AddImageHandler()
    {
        Images = Images.Concat(new string[] { "" }).ToArray();
        _backups = Images.DeepCopyByJson<string[]>();

        await OnImagesChanged.InvokeAsync(Images);
    }

    public async Task DeleteImageHandler(int imageIndex)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Do you really want to delete this image?" },
            { "ButtonText", "Delete" },
            { "Color", Color.Error }
        };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete image", parameters);
        var result = await dialog.Result;

        if (result.Canceled)
            return;

        List<string> imagesList = Images.ToList();
        imagesList.RemoveAt(imageIndex);
        Images = imagesList.ToArray();
        _backups = Images.DeepCopyByJson<string[]>();

        await OnImagesChanged.InvokeAsync(Images);
    }
}

