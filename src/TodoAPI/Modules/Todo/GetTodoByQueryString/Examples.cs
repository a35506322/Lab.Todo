namespace TodoAPI.Modules.Todo.GetTodoByQueryString;

public class GetTodoByQueryStringReqEx_NoFilter : IExampleProvider
{
    public object GetExample() => new GetTodoByQueryStringRequest();
}

public class GetTodoByQueryStringReqEx_WithTitle : IExampleProvider
{
    public object GetExample() => new GetTodoByQueryStringRequest { TodoTitle = "測試" };
}

public class GetTodoByQueryStringReqEx_WithIsComplete : IExampleProvider
{
    public object GetExample() => new GetTodoByQueryStringRequest { IsComplete = "Y" };
}

public class GetTodoByQueryStringReqEx_WithMultipleFilters : IExampleProvider
{
    public object GetExample() =>
        new GetTodoByQueryStringRequest
        {
            TodoTitle = "測試",
            IsComplete = "N",
            AddUserId = "testuser",
        };
}

public class GetTodoByQueryStringResEx_Ok_Success : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<IEnumerable<GetTodoByQueryStringResponse>>(
            Code.成功,
            "查詢成功",
            new List<GetTodoByQueryStringResponse>
            {
                new GetTodoByQueryStringResponse
                {
                    TodoId = 1,
                    TodoTitle = "測試待辦",
                    TodoContent = "測試內容",
                    IsComplete = "N",
                    CompleteTime = null,
                    AddTime = DateTime.Now,
                    AddUserId = "testuser",
                },
            }
        );
}
