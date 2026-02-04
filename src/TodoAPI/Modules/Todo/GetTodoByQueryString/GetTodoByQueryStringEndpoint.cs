namespace TodoAPI.Modules.Todo.GetTodoByQueryString;

public class GetTodoByQueryStringEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app.MapGet("/todo", Handler);

    [EndpointName("GetTodoByQueryString")]
    [EndpointSummary("查詢待辦事項清單")]
    [EndpointDescription("根據查詢條件查詢待辦事項清單")]
    [ProducesResponseType<APIResponse<IEnumerable<GetTodoByQueryStringResponse>>>(
        StatusCodes.Status200OK
    )]
    [RequestExample(typeof(GetTodoByQueryStringReqEx_NoFilter), "查詢待辦事項清單 - 無篩選條件")]
    [RequestExample(typeof(GetTodoByQueryStringReqEx_WithTitle), "查詢待辦事項清單 - 標題篩選")]
    [RequestExample(
        typeof(GetTodoByQueryStringReqEx_WithIsComplete),
        "查詢待辦事項清單 - 完成狀態篩選"
    )]
    [RequestExample(
        typeof(GetTodoByQueryStringReqEx_WithMultipleFilters),
        "查詢待辦事項清單 - 多條件篩選"
    )]
    [ResponseExample(
        StatusCodes.Status200OK,
        typeof(GetTodoByQueryStringResEx_Ok_Success),
        "查詢待辦事項清單成功"
    )]
    private static async Task<IResult> Handler(
        [AsParameters] GetTodoByQueryStringRequest request,
        LabContext context,
        CancellationToken cancellationToken
    )
    {
        var query = context.Todo.AsNoTracking();

        // 標題模糊搜尋
        if (!string.IsNullOrWhiteSpace(request.TodoTitle))
        {
            query = query.Where(x => x.TodoTitle.Contains(request.TodoTitle));
        }

        // 完成狀態篩選
        if (!string.IsNullOrWhiteSpace(request.IsComplete))
        {
            query = query.Where(x => x.IsComplete == request.IsComplete);
        }

        // 新增者篩選
        if (!string.IsNullOrWhiteSpace(request.AddUserId))
        {
            query = query.Where(x => x.AddUserId == request.AddUserId);
        }

        var todos = await query
            .Select(x => new GetTodoByQueryStringResponse
            {
                TodoId = x.TodoId,
                TodoTitle = x.TodoTitle,
                TodoContent = x.TodoContent,
                IsComplete = x.IsComplete,
                CompleteTime = x.CompleteTime,
                AddTime = x.AddTime,
                AddUserId = x.AddUserId,
            })
            .ToListAsync(cancellationToken);

        return APIResponseHelper.Ok(message: "查詢成功", data: todos);
    }
}
