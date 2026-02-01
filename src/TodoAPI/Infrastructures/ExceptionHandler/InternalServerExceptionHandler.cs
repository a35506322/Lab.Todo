namespace TodoAPI.Infrastructures.ExceptionHandler;

public class InternalServerExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var type = exception.GetType().Name;
        var title = exception.Message;
        var detail = exception.ToString();
        var requestId = httpContext.TraceIdentifier;

        var errorResponse = new APIResponse<object>(
            Code: Code.程式內部錯誤,
            Message: "程式內部錯誤",
            ExceptionDetails: new ExceptionDetails(
                Type: type,
                Title: title,
                Detail: detail,
                RequestId: requestId
            )
        );

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        return true;
    }
}
