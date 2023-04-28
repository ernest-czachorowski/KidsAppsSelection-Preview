namespace Presentation.Razor.Default.Components;

public partial class InfiniteScroller : IAsyncDisposable
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Parameter, EditorRequired]
    public EventCallback<bool> OnScrollerInViewPort { get; set; }

    [Parameter, EditorRequired]
    public bool IsActive { get; set; } = false;

    protected readonly string observerTargetId = Guid.NewGuid().ToString();

    private bool _isLoading = false;

    private DotNetObjectReference<InfiniteScroller> _thisObjRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _thisObjRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeAsync<dynamic>("ObserverHandler.addObserver", _thisObjRef, observerTargetId);
        }
    }

    [JSInvokable]
    public async Task OnIntersection()
    {
        if (_isLoading)
            return;

        if (!IsActive)
            return;

        _isLoading = true;
        await OnScrollerInViewPort.InvokeAsync();
        _isLoading = false;
    }

    public async ValueTask DisposeAsync()
    {
        await JSRuntime.InvokeVoidAsync("ObserverHandler.removeObserver", observerTargetId);
        _thisObjRef?.Dispose();
    }
}