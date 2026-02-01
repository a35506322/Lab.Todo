namespace TodoAPI.Infrastructures.Security;

public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult
    )
    {
        // 自訂義 token 驗證失敗回應
        if (authorizeResult.Challenged)
        {
            var failureReasons = authorizeResult?.AuthorizationFailure?.FailureReasons;
            string errMsg =
                failureReasons is not null && failureReasons.Any()
                    ? JsonSerializer.Serialize(failureReasons)
                    : "未驗證或 Token 無效";
            APIResponse<object> response = new(Code: Code.授權失敗, Message: errMsg);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        // 授權失敗
        if (authorizeResult.Forbidden)
        {
            var failureReasons = authorizeResult?.AuthorizationFailure?.FailureReasons;
            string errMsg =
                failureReasons is not null && failureReasons.Any()
                    ? JsonSerializer.Serialize(failureReasons)
                    : "權限不足";
            APIResponse<object> response = new(Code: Code.驗證權限失敗, Message: errMsg);
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        // Fall back to the default implementation.
        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
