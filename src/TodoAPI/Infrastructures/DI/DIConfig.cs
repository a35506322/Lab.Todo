namespace TodoAPI.Infrastructures.DI;

public static class DIConfig
{
    public static void AddDI(this IServiceCollection services)
    {
        services.AddSingleton<IJWTHelper, JWTHelper>();
        services.AddScoped<IJWTProfilerHelper, JWTProfilerHelper>();
    }
}
