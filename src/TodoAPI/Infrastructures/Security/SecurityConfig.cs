namespace TodoAPI.Infrastructures.Security;

public static class SecurityConfig
{
    public static void AddSecurity(this IServiceCollection services)
    {
        // 401 / 403 自定義回應
        services.AddSingleton<
            IAuthorizationMiddlewareResultHandler,
            CustomAuthorizationMiddlewareResultHandler
        >();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                PolicyNames.RequireAdminRole,
                policy => policy.RequireRole(RoleNames.Admin)
            );
        });
    }
}
