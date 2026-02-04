using TodoAPI.Infrastructures.ExceptionHandler.ValidationMessage;

namespace TodoAPI.Infrastructures.ExceptionHandler;

public static class ExceptionHandlerConfig
{
    public static void AddCustomExceptionHandle(this IServiceCollection services)
    {
        // Tips: AddExceptionHandler + AddProblemDetails 為一組，不能分開使用
        services.AddExceptionHandler<InternalServerExceptionHandler>();
        services.AddProblemDetails();

        // Tips: 如果需要自訂 400 驗證錯誤回傳格式，可以註冊 CustomProblemDetailsService
        // 參考文章: https://github.com/dotnet/aspnetcore/blob/main/src/Http/Http.Extensions/src/ProblemDetailsService.cs
        services.AddSingleton<IProblemDetailsService, CustomProblemDetailsService>();
    }
}
