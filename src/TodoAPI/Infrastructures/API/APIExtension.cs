namespace TodoAPI.Infrastructures.API;

public static class APIExtension
{
    public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.MapEndpoint(app);
        return app;
    }
}
