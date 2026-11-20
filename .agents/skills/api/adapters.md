# Adapters (第三方 API 封裝)

封裝第三方 API 呼叫，統一管理 HttpClient、SSL 處理、Log 記錄。

## 資料夾結構

```text
Infrastructures/
└── Adapters/
    ├── AdapterConfig.cs          # DI 註冊設定（統一註冊所有 Adapter）
    └── [XXX]/                    # 第三方 API 名稱（如 YouBike）
        ├── I[XXX]Adapter.cs      # 介面定義
        ├── [XXX]Adapter.cs       # 實作類別
        ├── [XXX]Dto.cs           # 資料傳輸物件（可多個）
        └── [XXX]Interceptor.cs   # HTTP 攔截器（SSL + Log）
```

## 命名規範

| 類型   | 命名規則           | 範例                  |
| ------ | ------------------ | --------------------- |
| 介面   | `I[XXX]Adapter`    | `IYouBikeAdapter`     |
| 實作   | `[XXX]Adapter`     | `YouBikeAdapter`      |
| DTO    | `[XXX][功能]Dto`   | `YouBikeImmediateDto` |
| 攔截器 | `[XXX]Interceptor` | `YouBikeInterceptor`  |

## 介面定義 (I[XXX]Adapter.cs)

定義第三方 API 的所有方法，並撰寫 XML 註解說明用途。

```csharp
namespace TodoAPI.Infrastructures.Adapters.[XXX];

public interface I[XXX]Adapter
{
    /// <summary>
    /// [方法說明]
    /// </summary>
    /// <returns>[回傳說明]</returns>
    Task<IEnumerable<[XXX]Dto>> Get[XXX]Async();
}
```

## 實作類別 ([XXX]Adapter.cs)

- 注入 `HttpClient` 和 `IConfiguration`，從設定檔讀取 BaseUrl
- **BaseAddress 結尾必須加 `/`**
- 使用 `GetFromJsonAsync` 或 `PostAsJsonAsync` 呼叫 API
- 使用 `JsonHelper` 進行 JSON 序列化與反序列化

```csharp
namespace TodoAPI.Infrastructures.Adapters.[XXX];

public class [XXX]Adapter : I[XXX]Adapter
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public [XXX]Adapter(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        // BaseAddress 結尾必須加 "/"
        _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("[XXX]:BaseUrl")!);
    }

    public async Task<IEnumerable<[XXX]Dto>> Get[XXX]Async()
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<[XXX]Dto>>(
            "api/endpoint/path"
        );
        return response ?? Enumerable.Empty<[XXX]Dto>();
    }
}
```

## DTO 類別 ([XXX]Dto.cs)

- 使用 `JsonPropertyName` 對應 API 回傳的 JSON 欄位
- 撰寫 XML 註解說明每個欄位用途
- 設定預設值避免 null

```csharp
using System.Text.Json.Serialization;

namespace TodoAPI.Infrastructures.Adapters.[XXX];

public class [XXX]Dto
{
    /// <summary>
    /// [欄位說明]
    /// </summary>
    [JsonPropertyName("fieldName")]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// [數值欄位說明]
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; } = 0;
}
```

## 攔截器 ([XXX]Interceptor.cs)

處理 SSL 憑證驗證與 HTTP Request/Response 的結構化 Log。

- 繼承 `DelegatingHandler`
- 以 `HttpClientHandler` 處理 SSL 憑證
- 記錄結構化 Log（包含 Request/Response 資訊）

```csharp
namespace TodoAPI.Infrastructures.Adapters.[XXX];

public class [XXX]Interceptor : DelegatingHandler
{
    private readonly ILogger<[XXX]Interceptor> _logger;

    public [XXX]Interceptor(ILogger<[XXX]Interceptor> logger)
        : base(
            new HttpClientHandler()
            {
                // 處理 SSL 憑證驗證（如需略過驗證）
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (
                    httpRequestMessage,
                    cert,
                    cetChain,
                    policyErrors
                ) => true,
            }
        )
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        // 準備請求資訊
        var requestHeaders = request.Headers.ToDictionary(
            h => h.Key,
            h => string.Join(", ", h.Value)
        );
        var requestContentHeaders = new Dictionary<string, string>();

        if (request.Content?.Headers != null)
        {
            requestContentHeaders = request.Content.Headers.ToDictionary(
                h => h.Key,
                h => string.Join(", ", h.Value)
            );
        }

        string requestPayload = string.Empty;
        if (request.Content != null)
        {
            string payload = await request.Content.ReadAsStringAsync();
            try
            {
                requestPayload = JsonHelper.ToJson(payload);
            }
            catch
            {
                requestPayload = payload;
            }
        }

        // 發送請求
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        // 準備回應資訊
        string responsePayload = await response.Content.ReadAsStringAsync();
        try
        {
            responsePayload = JsonHelper.ToJson(responsePayload);
        }
        catch
        {
            // 保持原始內容
        }

        // 記錄結構化 Log
        _logger.LogInformation(
            "Method: {Method}, RequestUri: {RequestUri}, RequestHeaders: {RequestHeaders}, RequestContentHeaders: {RequestContentHeaders}, RequestPayload: {RequestPayload}, StatusCode: {StatusCode}, ResponsePayload: {ResponsePayload}",
            request.Method,
            request.RequestUri?.ToString(),
            JsonHelper.ToJson(requestHeaders),
            JsonHelper.ToJson(requestContentHeaders),
            requestPayload,
            (int)response.StatusCode,
            responsePayload
        );

        return response;
    }
}
```

## DI 註冊設定 (AdapterConfig.cs)

在 `AdapterConfig.cs` 統一註冊所有 Adapter：

- 註冊 Interceptor 為 `Scoped`
- 使用 `AddHttpClient<TInterface, TImplementation>` 註冊 HttpClient
- 使用 `ConfigurePrimaryHttpMessageHandler` 設定攔截器
- 使用 `AddStandardResilienceHandler` 加入重試機制（需安裝 `Microsoft.Extensions.Http.Resilience`）

```csharp
namespace TodoAPI.Infrastructures.Adapters;

public static class AdapterConfig
{
    public static void AddAdapters(this IServiceCollection services)
    {
        // [XXX] Adapter
        services.AddScoped<[XXX]Interceptor>();
        services
            .AddHttpClient<I[XXX]Adapter, [XXX]Adapter>()
            .ConfigurePrimaryHttpMessageHandler<[XXX]Interceptor>()
            .AddStandardResilienceHandler();
    }
}
```

## appsettings.json 設定

```json
{
    "[XXX]": {
        "BaseUrl": "https://api.example.com/"
    }
}
```

## 在 Endpoint 中使用 Adapter

```csharp
private static async Task<IResult> Handler(
    I[XXX]Adapter adapter,
    CancellationToken cancellationToken
)
{
    var result = await adapter.Get[XXX]Async();
    return APIResponseHelper.Ok(message: "查詢成功", data: result);
}
```
