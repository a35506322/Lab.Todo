namespace TodoAPI.Infrastructures.API;

public enum Code
{
    成功 = 2000,
    查詢成功但無資料 = 2004,
    資料驗證錯誤 = 4000,
    商業邏輯錯誤 = 4022,
    程式內部錯誤 = 5000,
}

/// <summary>
/// API 統一回傳格式
/// </summary>
/// <typeparam name="T">回傳資料類型</typeparam>
/// <param name="Code">狀態碼</param>
/// <param name="Message">訊息</param>
/// <param name="Data">status code 200 回傳資料</param>
/// <param name="ValidationErrors">status code 400 資料驗證錯誤</param>
/// <param name="Exception">status code 500 例外錯誤</param>
public record APIResponse<T>(
    [Display(Name = "狀態碼")] Code Code,
    [Display(Name = "訊息")] string Message,
    [Display(Name = "回傳資料")] T? Data = default,
    [Display(Name = "資料驗證錯誤")] Dictionary<string, string[]>? ValidationErrors = null,
    [Display(Name = "例外錯誤")] Exception? Exception = null
);

public static class APIResponseHelper
{
    public static IResult Ok<T>(string message = "操作成功", T data = default) =>
        Results.Ok(new APIResponse<T>(Code: Code.成功, Message: message, Data: data));

    public static IResult BadRequest<T>(Dictionary<string, string[]> validationErrors) =>
        Results.BadRequest(
            new APIResponse<T>(
                Code: Code.資料驗證錯誤,
                Message: "資料驗證錯誤",
                ValidationErrors: validationErrors
            )
        );

    public static IResult BusinessLogicError<T>(string message) =>
        Results.UnprocessableEntity(new APIResponse<T>(Code: Code.商業邏輯錯誤, Message: message));

    public static IResult InternalServerError<T>(Exception exception) =>
        Results.InternalServerError(
            new APIResponse<T>(
                Code: Code.程式內部錯誤,
                Message: "程式內部錯誤",
                Exception: exception
            )
        );
}
