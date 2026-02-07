namespace TodoAPI.Modules.Todo.UpdateTodoById;

public class UpdateTodoByIdEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("/updateTodoById/{id:long}", Handler);

    [EndpointName("UpdateTodoById")]
    [EndpointSummary("更新待辦事項")]
    [EndpointDescription("根據 ID 更新待辦事項")]
    [ProducesResponseType<APIResponse<UpdateTodoByIdResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<APIResponse<UpdateTodoByIdResponse>>(
        StatusCodes.Status422UnprocessableEntity
    )]
    [RequestExample(typeof(UpdateTodoByIdReqEx_UpdateTitle), "更新單筆待辦事項 - 更新標題")]
    [RequestExample(typeof(UpdateTodoByIdReqEx_MarkComplete), "更新單筆待辦事項 - 標記完成")]
    [ResponseExample(
        StatusCodes.Status200OK,
        typeof(UpdateTodoByIdResEx_Ok_Success),
        "更新單筆待辦事項成功"
    )]
    [ResponseExample(
        StatusCodes.Status422UnprocessableEntity,
        typeof(UpdateTodoByIdResEx_422_TodoNotFound),
        "找不到指定的待辦事項"
    )]
    private static async Task<IResult> Handler(
        long id,
        UpdateTodoByIdRequest request,
        LabContext context,
        CancellationToken cancellationToken
    )
    {
        var todo = await context.Todo.FindAsync(new object[] { id }, cancellationToken);

        if (todo is null)
        {
            return APIResponseHelper.BusinessLogicError<UpdateTodoByIdResponse>(
                message: "找不到指定的待辦事項"
            );
        }

        var wasIncomplete = todo.IsComplete == "N";
        todo.TodoTitle = request.TodoTitle;
        todo.TodoContent = request.TodoContent;
        todo.IsComplete = request.IsComplete;

        // 如果從未完成變更為完成，設定完成時間
        if (wasIncomplete && request.IsComplete == "Y")
        {
            todo.CompleteTime = DateTime.Now;
        }
        // 如果從完成變更為未完成，清除完成時間
        else if (!wasIncomplete && request.IsComplete == "N")
        {
            todo.CompleteTime = null;
        }

        await context.SaveChangesAsync(cancellationToken);

        var response = new UpdateTodoByIdResponse
        {
            TodoId = todo.TodoId,
            TodoTitle = todo.TodoTitle,
            TodoContent = todo.TodoContent,
            IsComplete = todo.IsComplete,
            CompleteTime = todo.CompleteTime,
            AddTime = todo.AddTime,
            AddUserId = todo.AddUserId,
        };

        return APIResponseHelper.Ok(message: "更新成功", data: response);
    }
}
