using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Exceptions.MsSqlServer.Destructurers;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

namespace TodoAPI.Infrastructures.Logging;

public static class SerilogConfig
{
    public static void AddSerilLog(IConfiguration configuration, IWebHostEnvironment environment)
    {
        string logPath = configuration.GetValue<string>("SerilLogConfig:LogPath");
        // string seqUrl = configuration.GetValue<string>("SerilLogConfig:SeqUrl");

        // 全域設定
        /*  🔔new CompactJsonFormatter()
         *  由於 Log 的欄位很多，使用 Console Sink 會比較看不出來，改用 Serilog.Formatting.Compact 來記錄 JSON 格式的 Log 訊息會清楚很多！
         */
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override(
                "Microsoft.EntityFrameworkCore.Database.Command",
                LogEventLevel.Warning
            )
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "TodoAPI")
            .Enrich.WithCustomHttpContext()
            .Enrich.WithExceptionDetails(
                new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new[] { new SqlExceptionDestructurer() })
                    .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })
            )
            .WriteTo.Map(
                evt => evt.Timestamp.ToString("yyyyMM"),
                (month, wt) =>
                    wt.File(
                        new CompactJsonFormatter(),
                        path: Path.Combine(logPath, month, "log-.txt"),
                        rollOnFileSizeLimit: true,
                        rollingInterval: RollingInterval.Day,
                        shared: true
                    )
            )
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate: "[{Timestamp:yyyy/MM/dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{RequestBody}{ResponseBody}{Exception}"
            );
        //.WriteTo.Seq(seqUrl)

        Log.Logger = loggerConfiguration.CreateLogger();
    }

    private static readonly HashSet<string> _skipLoggingPaths = new() { "/scalar", "/openapi" };

    public static async void EnrichFromRequest(
        IDiagnosticContext diagnosticContext,
        HttpContext httpContext
    )
    {
        var request = httpContext.Request;
        var isSkipLogging = _skipLoggingPaths.Any(path => request.Path.StartsWithSegments(path));

        var requestBody = isSkipLogging
            ? string.Empty
            : httpContext.Items["RequestBody"]?.ToString() ?? string.Empty;
        diagnosticContext.Set("RequestBody", requestBody);

        var responseBody = await ReadResponseBody(httpContext.Response);
        var responseBodyLog = isSkipLogging ? string.Empty : responseBody;
        diagnosticContext.Set("ResponseBody", responseBodyLog);

        diagnosticContext.Set("Host", request.Host); // X-Forwarded-Host
        diagnosticContext.Set("Scheme", request.Scheme); // X-Forwarded-Proto
        diagnosticContext.Set("Prefix", request.PathBase); // X-Forwarded-Prefix
        diagnosticContext.Set("Headers", request.Headers);

        string ip =
            request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? httpContext?.Connection.RemoteIpAddress?.ToString()
            ?? "Unknown";
        diagnosticContext.Set("RemoteIp", ip);

        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }

        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

        var endpoint = httpContext.GetEndpoint();
        if (endpoint is object)
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }
    }

    private static async Task<string> ReadResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        string responseBody = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return $"{responseBody}";
    }
}
