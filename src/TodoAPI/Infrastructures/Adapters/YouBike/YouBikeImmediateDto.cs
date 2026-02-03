using System.Text.Json.Serialization;

namespace TodoAPI.Infrastructures.Adapters.YouBike;

public class YouBikeImmediateDto
{
    /// <summary>
    /// 站點編號
    /// </summary>
    [JsonPropertyName("sno")]
    public string Sno { get; set; } = string.Empty;

    /// <summary>
    /// 站點名稱
    /// </summary>
    [JsonPropertyName("sna")]
    public string Sna { get; set; } = string.Empty;

    /// <summary>
    /// 站點所在區域
    /// </summary>
    [JsonPropertyName("sarea")]
    public string Sarea { get; set; } = string.Empty;

    /// <summary>
    /// 資料更新時間
    /// </summary>
    [JsonPropertyName("mday")]
    public string Mday { get; set; } = string.Empty;

    /// <summary>
    /// 站點地址
    /// </summary>
    [JsonPropertyName("ar")]
    public string Ar { get; set; } = string.Empty;

    /// <summary>
    /// 站點所在區域的英文名稱
    /// </summary>
    [JsonPropertyName("sareaen")]
    public string Sareaen { get; set; } = string.Empty;

    /// <summary>
    /// 站點名稱的英文翻譯
    /// </summary>
    [JsonPropertyName("snaen")]
    public string Snaen { get; set; } = string.Empty;

    /// <summary>
    /// 站點地址的英文翻譯
    /// </summary>
    [JsonPropertyName("aren")]
    public string Aren { get; set; } = string.Empty;

    /// <summary>
    /// 站點是否在使用中
    /// </summary>
    [JsonPropertyName("act")]
    public string Act { get; set; } = string.Empty;

    /// <summary>
    /// 原始資料來源的更新時間
    /// </summary>
    [JsonPropertyName("srcUpdateTime")]
    public string SrcUpdateTime { get; set; } = string.Empty;

    /// <summary>
    /// 資料更新時間
    /// </summary>
    [JsonPropertyName("updateTime")]
    public string UpdateTime { get; set; } = string.Empty;

    /// <summary>
    /// 資訊時間
    /// </summary>
    [JsonPropertyName("infoTime")]
    public string InfoTime { get; set; } = string.Empty;

    /// <summary>
    /// 資訊日期
    /// </summary>
    [JsonPropertyName("infoDate")]
    public string InfoDate { get; set; } = string.Empty;

    /// <summary>
    /// 總車位數量
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 0;

    /// <summary>
    /// 可租借的腳踏車數量
    /// </summary>
    [JsonPropertyName("available_rent_bikes")]
    public int AvailableRentBikes { get; set; } = 0;

    /// <summary>
    /// 站點的緯度坐標
    /// </summary>
    [JsonPropertyName("latitude")]
    public float Latitude { get; set; } = 0;

    /// <summary>
    /// 站點的經度坐標
    /// </summary>
    [JsonPropertyName("longitude")]
    public float Longitude { get; set; } = 0;

    /// <summary>
    /// 可歸還的腳踏車數量
    /// </summary>
    [JsonPropertyName("available_return_bikes")]
    public int AvailableReturnBikes { get; set; } = 0;
}
