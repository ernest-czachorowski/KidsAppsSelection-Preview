namespace Presentation.Services;

public interface IResponseManagerService
{
    public Task<ServiceResponse<T>> Parse<T>(HttpResponseMessage? httpResponse);
}
