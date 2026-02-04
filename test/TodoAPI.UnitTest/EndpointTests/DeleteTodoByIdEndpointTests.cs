using TodoAPI.Modules.Todo.DeleteTodoById;

namespace TodoAPI.UnitTest.EndpointTests;

[TestClass]
public class DeleteTodoByIdEndpointTests
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
    public async Task Handler_ValidId_ReturnsOk()
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

        // Act
        var result = await DeleteTodoByIdEndpoint.Handler(todoId, _context, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(Ok<APIResponse<DeleteTodoByIdResponse>>));

        var okResult = (Ok<APIResponse<DeleteTodoByIdResponse>>)result;
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(Code.成功, okResult.Value.Code);
        Assert.AreEqual("刪除成功", okResult.Value.Message);

        // 驗證資料已刪除
        var deletedTodo = await _context.Todo.FindAsync(todoId);
        Assert.IsNull(deletedTodo);
    }

    [TestMethod]
    public async Task Handler_InvalidId_ReturnsBusinessLogicError()
    {
        // Arrange
        var invalidId = 999L;

        // Act
        var result = await DeleteTodoByIdEndpoint.Handler(
            invalidId,
            _context,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(UnprocessableEntity<APIResponse<DeleteTodoByIdResponse>>)
        );

        var errorResult = (UnprocessableEntity<APIResponse<DeleteTodoByIdResponse>>)result;
        Assert.IsNotNull(errorResult.Value);
        Assert.AreEqual(Code.商業邏輯錯誤, errorResult.Value.Code);
        Assert.AreEqual("找不到指定的待辦事項", errorResult.Value.Message);
    }
}
