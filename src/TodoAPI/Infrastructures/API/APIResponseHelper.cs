namespace TodoAPI.Infrastructures.API;

public enum Code
{
    成功 = 2000,
    查詢成功但無資料 = 2004,
    資料驗證錯誤 = 4000,
    授權失敗 = 4001,
    驗證權限失敗 = 4003,
    商業邏輯錯誤 = 4022,
    程式內部錯誤 = 5000,
}

/// <summary>
/// 例外錯誤詳細資訊
/// </summary>
/// <param name="Type">例外錯誤類型</param>
/// <param name="Title">例外錯誤標題</param>
/// <param name="Detail">例外錯誤詳細資訊</param>
/// <param name="RequestId">請求 ID</param>
public record ExceptionDetails(string Type, string Title, string Detail, string? RequestId = null);

/// <summary>
/// API 統一回傳格式
/// </summary>
/// <typeparam name="T">回傳資料類型</typeparam>
/// <param name="Code">狀態碼</param>
/// <param name="Message">訊息</param>
/// <param name="Data">status code 200 回傳資料</param>
/// <param name="ValidationErrors">status code 400 資料驗證錯誤</param>
/// <param name="Exception">status code 500 例外錯誤</param>
/// <param name="TraceId">追蹤 ID</param>
///
public record APIResponse<T>
{
    public Code Code { get; }
    public string Message { get; }
    public T? Data { get; }
    public Dictionary<string, string[]>? ValidationErrors { get; }
    public ExceptionDetails? ExceptionDetails { get; }
    public string TraceId { get; }

    public APIResponse(
        [Display(Name = "狀態碼")] Code Code,
        [Display(Name = "訊息")] string Message,
        [Display(Name = "回傳資料")] T? Data = default,
        [Display(Name = "資料驗證錯誤")] Dictionary<string, string[]>? ValidationErrors = null,
        [Display(Name = "例外錯誤")] ExceptionDetails? ExceptionDetails = null,
        [Display(Name = "追蹤 ID")] string? TraceId = null
    )
    {
        this.Code = Code;
        this.Message = Message;
        this.Data = Data;
        this.ValidationErrors = ValidationErrors;
        this.ExceptionDetails = ExceptionDetails;
        this.TraceId = TraceId ?? Activity.Current?.Id ?? "";
    }
}

/// <summary>
/// API 統一回應 Helper
/// </summary>
/// <remarks>
/// <para>
/// Handler 需搭配 <c>Results&lt;T1, T2, ...&gt;</c> union type 宣告返回類型，
/// 否則類型信息會在返回 IResult 時丟失，導致 Scalar/Swagger Response Schema 無法正確顯示。
/// </para>
/// <para>
/// 可改採 Attributes 設定 Response Type，範例：
/// <code><![CDATA[
/// [ProducesResponseType<APIResponse<LoginResponse>>(StatusCodes.Status200OK)]
/// [ProducesResponseType<APIResponse<LoginResponse>>(StatusCodes.Status422UnprocessableEntity)]
/// ]]></code>
/// </para>
/// </remarks>
public static class APIResponseHelper
{
    public static Ok<APIResponse<T>> Ok<T>(string message = "操作成功", T data = default!) =>
        TypedResults.Ok(new APIResponse<T>(Code: Code.成功, Message: message, Data: data));

    public static BadRequest<APIResponse<T>> BadRequest<T>(
        Dictionary<string, string[]> validationErrors
    ) =>
        TypedResults.BadRequest(
            new APIResponse<T>(
                Code: Code.資料驗證錯誤,
                Message: "資料驗證錯誤",
                ValidationErrors: validationErrors
            )
        );

    public static UnprocessableEntity<APIResponse<T>> BusinessLogicError<T>(string message) =>
        TypedResults.UnprocessableEntity(
            new APIResponse<T>(Code: Code.商業邏輯錯誤, Message: message)
        );

    public static InternalServerError<APIResponse<T>> InternalServerError<T>(
        ExceptionDetails exceptionDetails
    ) =>
        TypedResults.InternalServerError(
            new APIResponse<T>(
                Code: Code.程式內部錯誤,
                Message: "程式內部錯誤",
                ExceptionDetails: exceptionDetails
            )
        );
}
