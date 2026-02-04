namespace TodoAPI.Modules.Todo.GetTodoById;

public class GetTodoByIdEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/todo/{id:long}", Handler);

    [EndpointName("GetTodoById")]
    [EndpointSummary("查詢待辦事項")]
    [EndpointDescription("根據 ID 查詢單筆待辦事項")]
    [ProducesResponseType<APIResponse<GetTodoByIdResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<APIResponse<GetTodoByIdResponse>>(
        StatusCodes.Status422UnprocessableEntity
    )]
    [ResponseExample(
        StatusCodes.Status200OK,
        typeof(GetTodoByIdResEx_Ok_Success),
        "查詢單筆待辦事項成功"
    )]
    [ResponseExample(
        StatusCodes.Status422UnprocessableEntity,
        typeof(GetTodoByIdResEx_422_TodoNotFound),
        "找不到指定的待辦事項"
    )]
    private static async Task<IResult> Handler(
        long id,
        LabContext context,
        CancellationToken cancellationToken
    )
    {
        var todo = await context
            .Todo.AsNoTracking()
            .Where(x => x.TodoId == id)
            .Select(x => new GetTodoByIdResponse
            {
                TodoId = x.TodoId,
                TodoTitle = x.TodoTitle,
                TodoContent = x.TodoContent,
                IsComplete = x.IsComplete,
                CompleteTime = x.CompleteTime,
                AddTime = x.AddTime,
                AddUserId = x.AddUserId,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (todo is null)
        {
            return APIResponseHelper.BusinessLogicError<GetTodoByIdResponse>(
                message: "找不到指定的待辦事項"
            );
        }

        return APIResponseHelper.Ok(message: "查詢成功", data: todo);
    }
}
