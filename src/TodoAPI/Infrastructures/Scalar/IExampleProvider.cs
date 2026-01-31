namespace TodoAPI.Infrastructures.Scalar;

/// <summary>
/// 提供某個 request 或 response 範例（給 OpenAPI/Scalar 用）.
/// </summary>
public interface IExampleProvider
{
    /// <summary>
    /// 回傳 request 或 response 範例物件，會再被序列化成 JSON 填進 OpenAPI example.
    /// </summary>
    object GetExample();
}
