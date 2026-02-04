namespace TodoAPI.Modules.Todo.DeleteTodoById;

public class DeleteTodoByIdResEx_Ok_Success : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<DeleteTodoByIdResponse>(
            Code.成功,
            "刪除成功",
            new DeleteTodoByIdResponse { TodoId = 1 }
        );
}

public class DeleteTodoByIdResEx_422_TodoNotFound : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<DeleteTodoByIdResponse>(Code.商業邏輯錯誤, "找不到指定的待辦事項", default);
}
