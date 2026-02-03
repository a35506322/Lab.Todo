namespace TodoAPI.Infrastructures.Adapters;

public static class AdapterConfig
{
    public static void AddAdapters(this IServiceCollection services)
    {
        services.AddScoped<YouBikeInterceptor>();
        services
            .AddHttpClient<IYouBikeAdapter, YouBikeAdapter>()
            .ConfigurePrimaryHttpMessageHandler<YouBikeInterceptor>()
            .AddStandardResilienceHandler();
    }
}
