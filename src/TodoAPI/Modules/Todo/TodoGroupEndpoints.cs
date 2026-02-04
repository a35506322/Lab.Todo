using TodoAPI.Modules.Todo.DeleteTodoById;
using TodoAPI.Modules.Todo.GetTodoById;
using TodoAPI.Modules.Todo.GetTodoByQueryString;
using TodoAPI.Modules.Todo.InsertTodo;
using TodoAPI.Modules.Todo.UpdateTodoById;

namespace TodoAPI.Modules.Todo;

public static class TodoGroupEndpoints
{
    public static void MapTodoGroupEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder todoEndpoints = app.MapGroup("/todo")
            .WithTags("Todo")
            .RequireAuthorization(PolicyNames.RequireAdminRole);
        todoEndpoints.MapEndpoint<GetTodoByIdEndpoint>();
        todoEndpoints.MapEndpoint<InsertTodoEndpoint>();
        todoEndpoints.MapEndpoint<UpdateTodoByIdEndpoint>();
        todoEndpoints.MapEndpoint<DeleteTodoByIdEndpoint>();
        todoEndpoints.MapEndpoint<GetTodoByQueryStringEndpoint>();
    }
}
