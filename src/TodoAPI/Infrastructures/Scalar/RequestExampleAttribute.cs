namespace TodoAPI.Infrastructures.Scalar;

/// <summary>
/// 標在 endpoint handler 上，指定 request body 的範例由哪個 <see cref="IExampleProvider"/> 提供.
/// 同一個 endpoint 可標多個，用 <paramref name="name"/> 區分，Scalar 會顯示為下拉選單.
/// </summary>
/// <param name="exampleProviderType">實作 <see cref="IExampleProvider"/> 的類型（回傳 request DTO）</param>
/// <param name="name">範例顯示名稱（可選）。多個 request 範例時用來區分</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequestExampleAttribute(Type exampleProviderType, string? name = null)
    : Attribute
{
    public Type ExampleProviderType { get; } = exampleProviderType;

    /// <summary>
    /// 範例顯示名稱。多個 request 範例時用來區分（如「一般登入」「測試帳號」）.
    /// </summary>
    public string? Name { get; } = name;
}
