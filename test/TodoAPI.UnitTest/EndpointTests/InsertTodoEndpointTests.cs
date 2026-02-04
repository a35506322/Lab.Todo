using TodoAPI.Modules.Todo.InsertTodo;

namespace TodoAPI.UnitTest.EndpointTests;

[TestClass]
public class InsertTodoEndpointTests
{
    private LabContext _context = null!;
    private IJWTProfilerHelper _jwtProfilerHelper = null!;

    [TestInitialize]
    public void Setup()
    {
        // 建立 In-Memory 資料庫
        var options = new DbContextOptionsBuilder<LabContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new LabContext(options);

        // 建立 Mock JWT Profiler Helper
        _jwtProfilerHelper = Substitute.For<IJWTProfilerHelper>();
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
        var userId = "testuser";
        var todoTitle = "測試待辦";
        var todoContent = "測試內容";

        _jwtProfilerHelper.UserId.Returns(userId);

        var request = new InsertTodoRequest { TodoTitle = todoTitle, TodoContent = todoContent };

        // Act
        var result = await InsertTodoEndpoint.Handler(
            request,
            _context,
            _jwtProfilerHelper,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(result, typeof(Ok<APIResponse<InsertTodoResponse>>));

        var okResult = (Ok<APIResponse<InsertTodoResponse>>)result;
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(Code.成功, okResult.Value.Code);
        Assert.AreEqual("新增成功", okResult.Value.Message);
        Assert.IsNotNull(okResult.Value.Data);
        Assert.IsTrue(okResult.Value.Data!.TodoId > 0);
        Assert.AreEqual(todoTitle, okResult.Value.Data!.TodoTitle);
        Assert.AreEqual(todoContent, okResult.Value.Data!.TodoContent);
        Assert.AreEqual("N", okResult.Value.Data!.IsComplete);
        Assert.AreEqual(userId, okResult.Value.Data!.AddUserId);

        // 驗證資料已儲存到資料庫
        var savedTodo = await _context.Todo.FindAsync(okResult.Value.Data!.TodoId);
        Assert.IsNotNull(savedTodo);
        Assert.AreEqual(todoTitle, savedTodo!.TodoTitle);
        Assert.AreEqual(userId, savedTodo.AddUserId);
    }
}
