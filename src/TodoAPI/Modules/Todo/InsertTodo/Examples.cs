namespace TodoAPI.Modules.Todo.InsertTodo;

public class InsertTodoReqEx_Basic : IExampleProvider
{
    public object GetExample() =>
        new InsertTodoRequest { TodoTitle = "測試待辦", TodoContent = "測試內容" };
}

public class InsertTodoReqEx_OnlyTitle : IExampleProvider
{
    public object GetExample() => new InsertTodoRequest { TodoTitle = "測試待辦" };
}

public class InsertTodoResEx_Ok_Success : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<InsertTodoResponse>(
            Code.成功,
            "新增成功",
            new InsertTodoResponse
            {
                TodoId = 1,
                TodoTitle = "測試待辦",
                TodoContent = "測試內容",
                IsComplete = "N",
                AddTime = DateTime.Now,
                AddUserId = "testuser",
            }
        );
}
