namespace TodoAPI.Modules.Todo.DeleteTodoById;

public class DeleteTodoByIdEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("/todo/{id:long}", Handler);

    [EndpointName("DeleteTodoById")]
    [EndpointSummary("刪除待辦事項")]
    [EndpointDescription("根據 ID 刪除待辦事項")]
    [ProducesResponseType<APIResponse<DeleteTodoByIdResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<APIResponse<DeleteTodoByIdResponse>>(
        StatusCodes.Status422UnprocessableEntity
    )]
    [ResponseExample(
        StatusCodes.Status200OK,
        typeof(DeleteTodoByIdResEx_Ok_Success),
        "刪除單筆待辦事項成功"
    )]
    [ResponseExample(
        StatusCodes.Status422UnprocessableEntity,
        typeof(DeleteTodoByIdResEx_422_TodoNotFound),
        "找不到指定的待辦事項"
    )]
    private static async Task<IResult> Handler(
        long id,
        LabContext context,
        CancellationToken cancellationToken
    )
    {
        var todo = await context.Todo.FindAsync(new object[] { id }, cancellationToken);

        if (todo is null)
        {
            return APIResponseHelper.BusinessLogicError<DeleteTodoByIdResponse>(
                message: "找不到指定的待辦事項"
            );
        }

        context.Todo.Remove(todo);
        await context.SaveChangesAsync(cancellationToken);

        var response = new DeleteTodoByIdResponse { TodoId = id };

        return APIResponseHelper.Ok(message: "刪除成功", data: response);
    }
}
