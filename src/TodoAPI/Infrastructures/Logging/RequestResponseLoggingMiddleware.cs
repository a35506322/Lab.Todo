namespace TodoAPI.Infrastructures.Logging;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // 讀取並記錄 request body data
        string requestBodyPayload = await ReadRequestBody(context.Request);
        context.Items["RequestBody"] = requestBodyPayload;

        // 讀取並記錄 response body data
        // 複製原始回應主體串流的指標
        var originalResponseBodyStream = context.Response.Body;

        // 建立新的記憶體串流...
        using (var responseBody = new MemoryStream())
        {
            //  ...並將其用於臨時回應主體
            context.Response.Body = responseBody;

            // 繼續沿著中介軟體管道向下執行，最終返回此類別
            await _next(context);

            // 將新記憶體串流（包含回應）的內容複製到原始串流，然後返回給客戶端。
            await responseBody.CopyToAsync(originalResponseBodyStream);
        }
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();

        var body = request.Body;
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        string requestBody = Encoding.UTF8.GetString(buffer);
        body.Seek(0, SeekOrigin.Begin);
        request.Body = body;

        return $"{requestBody}";
    }
}
