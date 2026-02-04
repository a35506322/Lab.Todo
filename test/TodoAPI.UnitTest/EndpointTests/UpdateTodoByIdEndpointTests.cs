using TodoAPI.Modules.Todo.UpdateTodoById;

namespace TodoAPI.UnitTest.EndpointTests;

[TestClass]
public class UpdateTodoByIdEndpointTests
{
    private LabContext _context = null!;

    [TestInitialize]
    public void Setup()
    {
        // 建立 In-Memory 資料庫
        var options = new DbContextOptionsBuilder<LabContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new LabContext(options);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
    }

    [TestMethod]
    public async Task Handler_ValidRequest_ReturnsOk()
    {
        // Arrange
        var todoId = 1L;
        var originalTitle = "原始標題";
        var newTitle = "新標題";
        var newContent = "新內容";

        var todo = new Todo
        {
            TodoId = todoId,
            TodoTitle = originalTitle,
            TodoContent = "原始內容",
            IsComplete = "N",
            AddUserId = "testuser",
            AddTime = DateTime.Now,
        };
        _context.Todo.Add(todo);
        await _context.SaveChangesAsync();

        var request = new UpdateTodoByIdRequest
        {
            TodoTitle = newTitle,
            TodoContent = newContent,
            IsComplete = "N",
        };

        // Act
        var result = await UpdateTodoByIdEndpoint.Handler(
            todoId,
            request,
            _context,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(result, typeof(Ok<APIResponse<UpdateTodoByIdResponse>>));

        var okResult = (Ok<APIResponse<UpdateTodoByIdResponse>>)result;
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(Code.成功, okResult.Value.Code);
        Assert.AreEqual("更新成功", okResult.Value.Message);
        Assert.IsNotNull(okResult.Value.Data);
        Assert.AreEqual(newTitle, okResult.Value.Data!.TodoTitle);
        Assert.AreEqual(newContent, okResult.Value.Data!.TodoContent);

        // 驗證資料已更新
        var updatedTodo = await _context.Todo.FindAsync(todoId);
        Assert.IsNotNull(updatedTodo);
        Assert.AreEqual(newTitle, updatedTodo!.TodoTitle);
        Assert.AreEqual(newContent, updatedTodo.TodoContent);
    }

    [TestMethod]
    public async Task Handler_InvalidId_ReturnsBusinessLogicError()
    {
        // Arrange
        var invalidId = 999L;
        var request = new UpdateTodoByIdRequest
        {
            TodoTitle = "新標題",
            TodoContent = "新內容",
            IsComplete = "N",
        };

        // Act
        var result = await UpdateTodoByIdEndpoint.Handler(
            invalidId,
            request,
            _context,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(UnprocessableEntity<APIResponse<UpdateTodoByIdResponse>>)
        );

        var errorResult = (UnprocessableEntity<APIResponse<UpdateTodoByIdResponse>>)result;
        Assert.IsNotNull(errorResult.Value);
        Assert.AreEqual(Code.商業邏輯錯誤, errorResult.Value.Code);
        Assert.AreEqual("找不到指定的待辦事項", errorResult.Value.Message);
    }

    [TestMethod]
    public async Task Handler_UpdateToComplete_UpdatesCompleteTime()
    {
        // Arrange
        var todoId = 1L;
        var todo = new Todo
        {
            TodoId = todoId,
            TodoTitle = "測試待辦",
            TodoContent = "測試內容",
            IsComplete = "N",
            AddUserId = "testuser",
            AddTime = DateTime.Now,
        };
        _context.Todo.Add(todo);
        await _context.SaveChangesAsync();

        var request = new UpdateTodoByIdRequest
        {
            TodoTitle = "測試待辦",
            TodoContent = "測試內容",
            IsComplete = "Y",
        };

        // Act
        var result = await UpdateTodoByIdEndpoint.Handler(
            todoId,
            request,
            _context,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(result, typeof(Ok<APIResponse<UpdateTodoByIdResponse>>));

        // 驗證完成時間已設定
        var updatedTodo = await _context.Todo.FindAsync(todoId);
        Assert.IsNotNull(updatedTodo);
        Assert.AreEqual("Y", updatedTodo!.IsComplete);
        Assert.IsNotNull(updatedTodo.CompleteTime);
    }
}
