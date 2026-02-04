using TodoAPI.Modules.Todo;

namespace TodoAPI.Infrastructures.API;

public static class APIExtension
{
    public static void MapGroupEndpoints(this WebApplication app)
    {
        RouteGroupBuilder endpoints = app.MapGroup("/api");
        endpoints.MapUserGroupEndpoints();
        endpoints.MapTodoGroupEndpoints();
    }

    public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.MapEndpoint(app);
        return app;
    }
}
