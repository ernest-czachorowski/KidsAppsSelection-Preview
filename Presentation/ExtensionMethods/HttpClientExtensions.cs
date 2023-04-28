namespace Presentation.ExtensionMethods;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage?> SendRequestAsync(Func<Task<HttpResponseMessage?>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception e)
        {
            await Console.Out.WriteLineAsync($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");
            return null;
        }
    }

    public static async Task<HttpResponseMessage?> TryDeleteAsync(this HttpClient self, string endPoint)
        => await SendRequestAsync(async () => await self.DeleteAsync(endPoint));

    public static async Task<HttpResponseMessage?> TryGetAsync(this HttpClient self, string endPoint)
        => await SendRequestAsync(async () => await self.GetAsync(endPoint));

    public static async Task<HttpResponseMessage?> TryPatchAsync<T>(this HttpClient self, string endPoint, T request)
        => await SendRequestAsync(async () => await self.PatchAsJsonAsync(endPoint, request));

    public static async Task<HttpResponseMessage?> TryPostAsync<T>(this HttpClient self, string endPoint, T request)
        => await SendRequestAsync(async () => await self.PostAsJsonAsync(endPoint, request));

    public static async Task<HttpResponseMessage?> TryPutAsync<T>(this HttpClient self, string endPoint, T request)
        => await SendRequestAsync(async () => await self.PutAsJsonAsync(endPoint, request));
}

