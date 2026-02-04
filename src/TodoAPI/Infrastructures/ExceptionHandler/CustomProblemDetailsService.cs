using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using TodoAPI.Infrastructures.ExceptionHandler.ValidationMessage;

namespace TodoAPI.Infrastructures.ExceptionHandler;

/// <summary>
/// 自訂 IProblemDetailsService，將 400 驗證錯誤改為回傳 APIResponse 格式。
/// </summary>
public sealed class CustomProblemDetailsService(
    IOptions<ProblemDetailsOptions> options,
    IEnumerable<IProblemDetailsWriter> writers,
    IWebHostEnvironment environment
) : IProblemDetailsService
{
    /// <inheritdoc />
    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        if (context.ProblemDetails is HttpValidationProblemDetails validationDetails)
        {
            return WriteValidationErrorResponse(
                context.HttpContext,
                validationDetails,
                environment
            );
        }

        return WriteDefaultProblemDetails(context);
    }

    private async ValueTask WriteValidationErrorResponse(
        HttpContext httpContext,
        HttpValidationProblemDetails validationDetails,
        IWebHostEnvironment environment
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
        if (environment.IsDevelopment())
        {
            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
            };
            await httpContext.Response.WriteAsJsonAsync(apiResponse, jsonOptions);
        }
        else
        {
            await httpContext.Response.WriteAsJsonAsync(apiResponse);
        }
    }

    private async ValueTask WriteDefaultProblemDetails(ProblemDetailsContext context)
    {
        options.Value.CustomizeProblemDetails?.Invoke(context);

        foreach (var writer in writers)
        {
            if (writer.CanWrite(context))
            {
                await writer.WriteAsync(context);
                return;
            }
        }

        throw new InvalidOperationException(
            "No registered IProblemDetailsWriter can write the response."
        );
    }
}
