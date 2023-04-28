namespace Application.Core;

public static class HttpResponse<T>
{
    public static (HttpStatusCode, ServiceResponse<T>) InternalServerError(string message = "Internal server error.")
        => (HttpStatusCode.InternalServerError, new ServiceResponse<T> { Success = false, Message = message });

    public static (HttpStatusCode, ServiceResponse<T>) NotFound(string message = "Not found.")
        => (HttpStatusCode.NotFound, new ServiceResponse<T> { Success = false, Message = message });

    public static (HttpStatusCode, ServiceResponse<T>) BadRequest(string message = "Bad request.")
        => (HttpStatusCode.BadRequest, new ServiceResponse<T> { Success = false, Message = message });

    public static (HttpStatusCode, ServiceResponse<T>) Confilct(string message = "Confilct.")
        => (HttpStatusCode.Conflict, new ServiceResponse<T> { Success = false, Message = message });

    public static (HttpStatusCode, ServiceResponse<T>) Unauthorized(string message = "Unauthorized.")
        => (HttpStatusCode.Unauthorized, new ServiceResponse<T> { Success = false, Message = message });

    public static (HttpStatusCode, ServiceResponse<T>) OK(T data, string message = "Ok.")
        => (HttpStatusCode.OK, new ServiceResponse<T> { Success = true, Data = data, Message = message });

    public static (HttpStatusCode, ServiceResponse<T>) Created(T data, string message = "Created.")
        => (HttpStatusCode.Created, new ServiceResponse<T> { Success = true, Data = data, Message = message });
}
