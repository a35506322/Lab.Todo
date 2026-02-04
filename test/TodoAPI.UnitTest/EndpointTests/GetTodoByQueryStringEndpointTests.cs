using TodoAPI.Modules.Todo.GetTodoByQueryString;

namespace TodoAPI.UnitTest.EndpointTests;

[TestClass]
public class GetTodoByQueryStringEndpointTests
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
    public async Task Handler_NoFilters_ReturnsAllTodos()
    {
        // Arrange
        var todos = new List<Todo>
        {
            new Todo
            {
                TodoId = 1,
                TodoTitle = "待辦1",
                TodoContent = "內容1",
                IsComplete = "N",
                AddUserId = "user1",
                AddTime = DateTime.Now,
            },
            new Todo
            {
                TodoId = 2,
                TodoTitle = "待辦2",
                TodoContent = "內容2",
                IsComplete = "Y",
                AddUserId = "user2",
                AddTime = DateTime.Now,
            },
        };
        _context.Todo.AddRange(todos);
        await _context.SaveChangesAsync();

        var query = new GetTodoByQueryStringRequest();

        // Act
        var result = await GetTodoByQueryStringEndpoint.Handler(
            query,
            _context,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(Ok<APIResponse<List<GetTodoByQueryStringResponse>>>)
        );

        var okResult = (Ok<APIResponse<List<GetTodoByQueryStringResponse>>>)result;
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(Code.成功, okResult.Value.Code);
        Assert.AreEqual("查詢成功", okResult.Value.Message);
        Assert.IsNotNull(okResult.Value.Data);
        Assert.AreEqual(2, okResult.Value.Data!.Count());
    }

    [TestMethod]
    public async Task Handler_WithTitleFilter_ReturnsFilteredTodos()
    {
        // Arrange
        var todos = new List<Todo>
        {
            new Todo
            {
                TodoId = 1,
                TodoTitle = "測試待辦",
                TodoContent = "內容1",
                IsComplete = "N",
                AddUserId = "user1",
                AddTime = DateTime.Now,
            },
            new Todo
            {
                TodoId = 2,
                TodoTitle = "其他待辦",
                TodoContent = "內容2",
                IsComplete = "N",
                AddUserId = "user1",
                AddTime = DateTime.Now,
            },
        };
        _context.Todo.AddRange(todos);
        await _context.SaveChangesAsync();

        var query = new GetTodoByQueryStringRequest { TodoTitle = "測試" };

        // Act
        var result = await GetTodoByQueryStringEndpoint.Handler(
            query,
            _context,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(Ok<APIResponse<List<GetTodoByQueryStringResponse>>>)
        );

        var okResult = (Ok<APIResponse<List<GetTodoByQueryStringResponse>>>)result;
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(Code.成功, okResult.Value.Code);
        Assert.IsNotNull(okResult.Value.Data);
        Assert.AreEqual(1, okResult.Value.Data!.Count());
        Assert.AreEqual("測試待辦", okResult.Value.Data!.First().TodoTitle);
    }

    [TestMethod]
    public async Task Handler_WithIsCompleteFilter_ReturnsFilteredTodos()
    {
        // Arrange
        var todos = new List<Todo>
        {
            new Todo
            {
                TodoId = 1,
                TodoTitle = "待辦1",
                TodoContent = "內容1",
                IsComplete = "N",
                AddUserId = "user1",
                AddTime = DateTime.Now,
            },
            new Todo
            {
                TodoId = 2,
                TodoTitle = "待辦2",
                TodoContent = "內容2",
                IsComplete = "Y",
                AddUserId = "user1",
                AddTime = DateTime.Now,
            },
        };
        _context.Todo.AddRange(todos);
        await _context.SaveChangesAsync();

        var query = new GetTodoByQueryStringRequest { IsComplete = "Y" };

        // Act
        var result = await GetTodoByQueryStringEndpoint.Handler(
            query,
            _context,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(Ok<APIResponse<List<GetTodoByQueryStringResponse>>>)
        );

        var okResult = (Ok<APIResponse<List<GetTodoByQueryStringResponse>>>)result;
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(Code.成功, okResult.Value.Code);
        Assert.IsNotNull(okResult.Value.Data);
        Assert.AreEqual(1, okResult.Value.Data!.Count());
        Assert.AreEqual("Y", okResult.Value.Data!.First().IsComplete);
    }

    [TestMethod]
    public async Task Handler_WithMultipleFilters_ReturnsFilteredTodos()
    {
        // Arrange
        var todos = new List<Todo>
        {
            new Todo
            {
                TodoId = 1,
                TodoTitle = "測試待辦",
                TodoContent = "內容1",
                IsComplete = "N",
                AddUserId = "user1",
                AddTime = DateTime.Now,
            },
            new Todo
            {
                TodoId = 2,
                TodoTitle = "測試待辦2",
                TodoContent = "內容2",
                IsComplete = "Y",
                AddUserId = "user1",
                AddTime = DateTime.Now,
            },
            new Todo
            {
                TodoId = 3,
                TodoTitle = "其他待辦",
                TodoContent = "內容3",
                IsComplete = "N",
                AddUserId = "user2",
                AddTime = DateTime.Now,
            },
        };
        _context.Todo.AddRange(todos);
        await _context.SaveChangesAsync();

        var query = new GetTodoByQueryStringRequest
        {
            TodoTitle = "測試",
            IsComplete = "N",
            AddUserId = "user1",
        };

        // Act
        var result = await GetTodoByQueryStringEndpoint.Handler(
            query,
            _context,
            CancellationToken.None
        );

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(Ok<APIResponse<List<GetTodoByQueryStringResponse>>>)
        );

        var okResult = (Ok<APIResponse<List<GetTodoByQueryStringResponse>>>)result;
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(Code.成功, okResult.Value.Code);
        Assert.IsNotNull(okResult.Value.Data);
        Assert.AreEqual(1, okResult.Value.Data!.Count());
        Assert.AreEqual("測試待辦", okResult.Value.Data!.First().TodoTitle);
        Assert.AreEqual("N", okResult.Value.Data!.First().IsComplete);
        Assert.AreEqual("user1", okResult.Value.Data!.First().AddUserId);
    }
}
