using TodoAPI.Modules.Todo.GetTodoById;

namespace TodoAPI.UnitTest.EndpointTests;

[TestClass]
public class GetTodoByIdEndpointTests
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
        var todoTitle = "測試待辦";
        var todoContent = "測試內容";
        var addUserId = "testuser";

        var todo = new Todo
        {
            TodoId = todoId,
            TodoTitle = todoTitle,
            TodoContent = todoContent,
            IsComplete = "N",
            AddUserId = addUserId,
            AddTime = DateTime.Now,
        };
        _context.Todo.Add(todo);
        await _context.SaveChangesAsync();

        // Act
        var result = await GetTodoByIdEndpoint.Handler(todoId, _context, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(Ok<APIResponse<GetTodoByIdResponse>>));

        var okResult = (Ok<APIResponse<GetTodoByIdResponse>>)result;
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(Code.成功, okResult.Value.Code);
        Assert.AreEqual("查詢成功", okResult.Value.Message);
        Assert.IsNotNull(okResult.Value.Data);
        Assert.AreEqual(todoId, okResult.Value.Data!.TodoId);
        Assert.AreEqual(todoTitle, okResult.Value.Data!.TodoTitle);
        Assert.AreEqual(todoContent, okResult.Value.Data!.TodoContent);
    }

    [TestMethod]
    public async Task Handler_InvalidId_ReturnsBusinessLogicError()
    {
        // Arrange
        var invalidId = 999L;

        // Act
        var result = await GetTodoByIdEndpoint.Handler(invalidId, _context, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(UnprocessableEntity<APIResponse<GetTodoByIdResponse>>)
        );

        var errorResult = (UnprocessableEntity<APIResponse<GetTodoByIdResponse>>)result;
        Assert.IsNotNull(errorResult.Value);
        Assert.AreEqual(Code.商業邏輯錯誤, errorResult.Value.Code);
        Assert.AreEqual("找不到指定的待辦事項", errorResult.Value.Message);
    }
}
