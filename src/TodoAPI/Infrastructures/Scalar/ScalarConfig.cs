namespace TodoAPI.Infrastructures.Scalar;

public static class ScalarConfig
{
    public static void UseScalar(this WebApplication app)
    {
        // Tips: 文件 https://scalar.com/
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("Lab.TodoAPI")
                .WithTheme(ScalarTheme.Purple)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
    }
}
