namespace Presentation.Services;

public class ResponseManagerService : IResponseManagerService
{
    private readonly ITokenService _tokenService;

    public ResponseManagerService(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<ServiceResponse<T>> Parse<T>(HttpResponseMessage? httpResponse)
    {
        if (httpResponse == null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = "No response from server. Are you online? Firewall may be blocking."
            };
        }

        foreach(var authHeader in httpResponse.Headers.WwwAuthenticate)
        {
            if (authHeader.Parameter is not null && authHeader.Parameter.Contains("invalid_token"))
            {
                await _tokenService.RemoveToken();
                return new ServiceResponse<T>
                {
                    Success = false,
                    Message = "Your token is invalid. Please login again."
                };
            }
        }

        ServiceResponse<T>? serviceResponse;

        try
        {
            serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<T?>>();
        }
        catch (Exception e)
        {
            await Console.Out.WriteLineAsync($"Exception: {e.GetType()} from: {e.Source} message: {e.Message}");

            return new ServiceResponse<T>
            {
                Success = false,
                Message = "Cannot read the response as JSON."
            };
        }

        if (serviceResponse == null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = "Cannot fetch data from the server."
            };
        }

        if (!httpResponse.IsSuccessStatusCode)
        {
            serviceResponse.Success = false;
        }

        if (!httpResponse.IsSuccessStatusCode && string.IsNullOrEmpty(serviceResponse.Message))
        {
            serviceResponse.Message = httpResponse.StatusCode.ToString();
        }

        return serviceResponse;
    }
}
