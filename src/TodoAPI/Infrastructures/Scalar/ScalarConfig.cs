namespace TodoAPI.Infrastructures.Scalar;

public static class ScalarConfig
{
    public static void UseScalar(this WebApplication app)
    {
        // Tips: 文件 https://scalar.com/
        app.MapScalarApiReference(options =>
        {
            options
                //.WithTheme(ScalarTheme.Purple)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme)
                .AddHttpAuthentication(
                    JwtBearerDefaults.AuthenticationScheme,
                    auth =>
                    {
                        auth.Token = ""; // 使用者可在 Scalar UI 輸入
                    }
                );
        });
    }
}
