namespace TodoAPI.Infrastructures.Scalar;

/// <summary>
/// 標在 endpoint handler 上，指定該狀態碼的 response 範例由哪個 IExampleProvider 提供.
/// 同一個 status code 可標多個，用 <paramref name="name"/> 區分，Scalar 會顯示為下拉選單.
/// </summary>
/// <param name="statusCode">HTTP 狀態碼，例如 200、422</param>
/// <param name="exampleProviderType">實作 <see cref="IExampleProvider"/> 的類型</param>
/// <param name="name">範例顯示名稱（可選）。同一個 status 多個範例時必填，用於 Scalar 下拉區分</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ResponseExampleAttribute(
    int statusCode,
    Type exampleProviderType,
    string? name = null
) : Attribute
{
    public int StatusCode { get; } = statusCode;
    public Type ExampleProviderType { get; } = exampleProviderType;

    /// <summary>
    /// 範例顯示名稱。同一個 status code 多個範例時用來區分（如「登入成功」「登入成功空資料」）.
    /// </summary>
    public string? Name { get; } = name;
}
