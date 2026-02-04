namespace TodoAPI.Modules.Todo.GetTodoById;

public class GetTodoByIdResEx_Ok_Success : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<GetTodoByIdResponse>(
            Code.成功,
            "查詢成功",
            new GetTodoByIdResponse
            {
                TodoId = 1,
                TodoTitle = "測試待辦",
                TodoContent = "測試內容",
                IsComplete = "N",
                CompleteTime = null,
                AddTime = DateTime.Now,
                AddUserId = "testuser",
            }
        );
}

public class GetTodoByIdResEx_422_TodoNotFound : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<GetTodoByIdResponse>(Code.商業邏輯錯誤, "找不到指定的待辦事項", default);
}
