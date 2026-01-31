using TodoAPI.Modules.Auth.User;

namespace TodoAPI.Infrastructures.API;

public static class APIExtension
{
    public static void MapGroupEndpoints(this WebApplication app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/api").WithOpenApi();
        endpoints.MapUserGroupEndpoints();
    }

    public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.MapEndpoint(app);
        return app;
    }
}
