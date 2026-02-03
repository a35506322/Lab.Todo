using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace TodoAPI.Common.Helpers;

/// <summary>
/// JSON 序列化與反序列化輔助類別
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// 反序列化用的預設設定（不分大小寫）
    /// </summary>
    private static readonly JsonSerializerOptions DefaultDeserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    /// <summary>
    /// 序列化用的預設設定（中文不編碼）
    /// </summary>
    private static readonly JsonSerializerOptions DefaultSerializeOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// 將 JSON 字串反序列化為物件（屬性名稱不分大小寫）
    /// </summary>
    /// <typeparam name="T">目標型別</typeparam>
    /// <param name="json">JSON 字串</param>
    /// <param name="options">額外的 JSON 設定（可選）</param>
    /// <returns>反序列化後的物件</returns>
    public static T? FromJson<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, DefaultDeserializeOptions);

    /// <summary>
    /// 將物件序列化為 JSON 字串（中文不編碼）
    /// </summary>
    /// <param name="obj">要序列化的物件</param>
    /// <param name="options">額外的 JSON 設定（可選）</param>
    /// <returns>JSON 字串</returns>
    public static string ToJson(object obj) =>
        JsonSerializer.Serialize(obj, DefaultSerializeOptions);
}
