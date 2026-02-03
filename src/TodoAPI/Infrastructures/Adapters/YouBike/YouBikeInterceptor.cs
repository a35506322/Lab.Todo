namespace TodoAPI.Infrastructures.Adapters.YouBike;

public class YouBikeInterceptor : DelegatingHandler
{
    private readonly ILogger<YouBikeInterceptor> _logger;

    public YouBikeInterceptor(ILogger<YouBikeInterceptor> logger)
        : base(
            new HttpClientHandler()
            {
                // The SSL connection could not be established, see inner exception
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (
                    httpRequestMessage,
                    cert,
                    cetChain,
                    policyErrors
                ) =>
                {
                    return true;
                },
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

        // 記錄一筆結構化 Log
        _logger.LogInformation(
            "Method: {Method}, RequestUri: {RequestUri}, RequestHeaders: {RequestHeaders}, RequestContentHeaders: {RequestContentHeaders}, RequestPayload: {RequestPayload}, StatusCode: {StatusCode}, , ResponsePayload: {ResponsePayload}",
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
