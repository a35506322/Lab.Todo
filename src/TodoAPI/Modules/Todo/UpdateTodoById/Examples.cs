namespace TodoAPI.Modules.Todo.UpdateTodoById;

public class UpdateTodoByIdReqEx_UpdateTitle : IExampleProvider
{
    public object GetExample() =>
        new UpdateTodoByIdRequest
        {
            TodoTitle = "更新後的標題",
            TodoContent = "更新後的內容",
            IsComplete = "N",
        };
}

public class UpdateTodoByIdReqEx_MarkComplete : IExampleProvider
{
    public object GetExample() =>
        new UpdateTodoByIdRequest
        {
            TodoTitle = "測試待辦",
            TodoContent = "測試內容",
            IsComplete = "Y",
        };
}

public class UpdateTodoByIdResEx_Ok_Success : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<UpdateTodoByIdResponse>(
            Code.成功,
            "更新成功",
            new UpdateTodoByIdResponse
            {
                TodoId = 1,
                TodoTitle = "更新後的標題",
                TodoContent = "更新後的內容",
                IsComplete = "N",
                CompleteTime = null,
                AddTime = DateTime.Now,
                AddUserId = "testuser",
            }
        );
}

public class UpdateTodoByIdResEx_422_TodoNotFound : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<UpdateTodoByIdResponse>(Code.商業邏輯錯誤, "找不到指定的待辦事項", default);
}
