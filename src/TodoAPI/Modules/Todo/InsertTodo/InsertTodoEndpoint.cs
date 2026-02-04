namespace TodoAPI.Modules.Todo.InsertTodo;

public class InsertTodoEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app.MapPost("/todo", Handler);

    [EndpointName("InsertTodo")]
    [EndpointSummary("新增待辦事項")]
    [EndpointDescription("新增一筆待辦事項")]
    [ProducesResponseType<APIResponse<InsertTodoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<APIResponse<InsertTodoResponse>>(StatusCodes.Status400BadRequest)]
    [RequestExample(typeof(InsertTodoReqEx_Basic), "新增待辦事項 - 基本範例")]
    [RequestExample(typeof(InsertTodoReqEx_OnlyTitle), "新增待辦事項 - 僅標題")]
    [ResponseExample(
        StatusCodes.Status200OK,
        typeof(InsertTodoResEx_Ok_Success),
        "新增待辦事項成功"
    )]
    public static async Task<IResult> Handler(
        InsertTodoRequest request,
        LabContext context,
        IJWTProfilerHelper jwtProfilerHelper,
        CancellationToken cancellationToken
    )
    {
        var todo = new TodoAPI.Infrastructures.Data.Entities.Todo
        {
            TodoTitle = request.TodoTitle,
            TodoContent = request.TodoContent,
            IsComplete = "N",
            AddTime = DateTime.Now,
            AddUserId = jwtProfilerHelper.UserId,
        };

        context.Todo.Add(todo);
        await context.SaveChangesAsync(cancellationToken);

        var response = new InsertTodoResponse
        {
            TodoId = todo.TodoId,
            TodoTitle = todo.TodoTitle,
            TodoContent = todo.TodoContent,
            IsComplete = todo.IsComplete,
            AddTime = todo.AddTime,
            AddUserId = todo.AddUserId,
        };

        return APIResponseHelper.Ok(message: "新增成功", data: response);
    }
}
