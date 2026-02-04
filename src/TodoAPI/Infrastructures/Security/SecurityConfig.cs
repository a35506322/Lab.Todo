namespace TodoAPI.Infrastructures.Security;

public static class SecurityConfig
{
    public static void AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT settings
        services.AddSingleton<IJWTHelper, JWTHelper>();
        services.AddScoped<IJWTProfilerHelper, JWTProfilerHelper>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
                options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                    NameClaimType =
                        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                    // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetValue<string>("JwtSettings:Issuer"),
                    ValidateAudience = true,
                    ValidAudience = configuration.GetValue<string>("JwtSettings:Audience"),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration.GetValue<string>("JwtSettings:SignKey")
                        )
                    ),
                };
            });

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
