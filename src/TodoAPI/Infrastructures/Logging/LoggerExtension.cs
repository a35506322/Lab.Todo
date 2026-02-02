using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TodoAPI.Infrastructures.Logging;

public static class LoggerExtension
{
    /// <summary>
    /// 操作開始 - [操作名稱] 開始執行
    /// </summary>
    public static void LogOperationStart(
        this ILogger logger,
        string operationName,
        object? context = null
    )
    {
        logger.LogInformation(
            "[{OperationName}] 開始執行 | 上下文:{@Context}",
            operationName,
            context
        );
    }

    /// <summary>
    /// 操作開始 - [操作名稱] 開始執行
    /// </summary>
    public static void LogOperationEnd(
        this ILogger logger,
        string operationName,
        object? context = null
    )
    {
        logger.LogInformation(
            "[{OperationName}] 開始執行 | 上下文:{@Context}",
            operationName,
            context
        );
    }

    /// <summary>
    /// 操作失敗 - [操作名稱] 執行失敗，錯誤原因
    /// </summary>
    public static void LogOperationFailed(
        this ILogger logger,
        string operationName,
        Exception? ex = null,
        object? context = null
    )
    {
        logger.LogError(
            "[{OperationName}] 執行失敗 | 錯誤詳細資訊:{Exception} | 上下文:{@Context}",
            operationName,
            ex?.ToString(),
            context
        );
    }
}
