namespace TodoAPI.Infrastructures.ExceptionHandler;

/// <summary>
/// 自訂 IProblemDetailsService
/// </summary>
public sealed class CustomProblemDetailsService : IProblemDetailsService
{
    private readonly IProblemDetailsWriter[] _writers;

    public CustomProblemDetailsService(IEnumerable<IProblemDetailsWriter> writers)
    {
        _writers = writers.ToArray();
    }

    public async ValueTask WriteAsync(ProblemDetailsContext context)
    {
        if (!await TryWriteAsync(context))
        {
            throw new InvalidOperationException(
                "Unable to find a registered `IProblemDetailsWriter` that can write to the given context."
            );
        }
    }

    public async ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(context.ProblemDetails);
        ArgumentNullException.ThrowIfNull(context.HttpContext);

        // 處理 400 驗證錯誤，回傳自訂的 APIResponse 格式
        // 檢查是否為 HttpValidationProblemDetails 且有 Errors
        if (
            context.ProblemDetails is HttpValidationProblemDetails validationDetails
            && validationDetails.Errors.Count > 0
        )
        {
            await WriteValidationErrorResponse(context.HttpContext, validationDetails);
            return true;
        }

        // 其他錯誤使用預設的 writers 處理
        // Try to write using all registered writers
        // sequentially and stop at the first one that
        // `canWrite`.
        for (var i = 0; i < _writers.Length; i++)
        {
            var selectedWriter = _writers[i];
            if (selectedWriter.CanWrite(context))
            {
                await selectedWriter.WriteAsync(context);
                return true;
            }
        }

        return false;
    }

    private async ValueTask WriteValidationErrorResponse(
        HttpContext httpContext,
        HttpValidationProblemDetails validationDetails
    )
    {
        var errors = new Dictionary<string, string[]>();

        foreach (var kv in validationDetails.Errors)
        {
            if (kv.Value is { Length: > 0 })
            {
                errors[kv.Key] = kv.Value.Select(ValidationErrorLocalizer.Localize).ToArray();
            }
        }

        var apiResponse = new APIResponse<object>(
            Code: Code.資料驗證錯誤,
            Message: "資料驗證錯誤",
            ValidationErrors: errors
        );

        httpContext.Response.StatusCode = 400;
        await httpContext.Response.WriteAsJsonAsync(apiResponse);
    }
}
