using System.Net.Http.Headers;
using TodoAPI.IntegrationTest.Helpers;
using TodoAPI.Modules.Todo.GetTodoByQueryString;
using TodoAPI.Modules.Todo.InsertTodo;
using TodoAPI.Modules.Todo.UpdateTodoById;

namespace TodoAPI.IntegrationTest.Tests.Todo;

/// <summary>
/// 依查詢字串取得 Todo 端點整合測試
/// </summary>
[TestClass]
public class GetTodoByQueryStringEndpointTests
{
    private static CustomWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _token = null!;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        _factory = new CustomWebApplicationFactory<Program>();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _factory?.Dispose();
    }

    [TestInitialize]
    public async Task Setup()
    {
        _client = _factory.CreateClient();
        _token = await TestAuthHelper.GetTokenAsync(_client);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            _token
        );
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
    }

    /// <summary>
    /// 測試無篩選條件查詢，應返回所有 Todo
    /// </summary>
    [TestMethod]
    public async Task GetTodoByQueryString_NoFilters_ReturnsAllTodos()
    {
        // Arrange: 新增兩個 Todo
        var insertRequest1 = new InsertTodoRequest { TodoTitle = "待辦1", TodoContent = "內容1" };
        var insertRequest2 = new InsertTodoRequest { TodoTitle = "待辦2", TodoContent = "內容2" };

        await _client.PostAsJsonAsync("/api/todo/todo", insertRequest1);
        await _client.PostAsJsonAsync("/api/todo/todo", insertRequest2);

        // Act: 查詢所有 Todo
        var getResponse = await _client.GetAsync("/api/todo/todo");

        // Assert
        getResponse.EnsureSuccessStatusCode();

        var getApiResponse = await getResponse.Content.ReadFromJsonAsync<
            APIResponse<List<GetTodoByQueryStringResponse>>
        >();

        Assert.IsNotNull(getApiResponse);
        Assert.AreEqual(Code.成功, getApiResponse.Code);
        Assert.AreEqual("查詢成功", getApiResponse.Message);
        Assert.IsNotNull(getApiResponse.Data);
        Assert.IsTrue(getApiResponse.Data.Count >= 2);
    }

    /// <summary>
    /// 測試使用標題篩選條件查詢，應返回符合條件的 Todo
    /// </summary>
    [TestMethod]
    public async Task GetTodoByQueryString_WithTitleFilter_ReturnsFilteredTodos()
    {
        // Arrange: 新增兩個 Todo
        var insertRequest1 = new InsertTodoRequest
        {
            TodoTitle = "測試待辦",
            TodoContent = "內容1",
        };
        var insertRequest2 = new InsertTodoRequest
        {
            TodoTitle = "其他待辦",
            TodoContent = "內容2",
        };

        await _client.PostAsJsonAsync("/api/todo/todo", insertRequest1);
        await _client.PostAsJsonAsync("/api/todo/todo", insertRequest2);

        // Act: 使用標題篩選查詢
        var getResponse = await _client.GetAsync("/api/todo/todo?TodoTitle=測試");

        // Assert
        getResponse.EnsureSuccessStatusCode();

        var getApiResponse = await getResponse.Content.ReadFromJsonAsync<
            APIResponse<List<GetTodoByQueryStringResponse>>
        >();

        Assert.IsNotNull(getApiResponse);
        Assert.AreEqual(Code.成功, getApiResponse.Code);
        Assert.IsNotNull(getApiResponse.Data);
        Assert.IsTrue(getApiResponse.Data.Count >= 1);
        Assert.IsTrue(
            getApiResponse.Data.Any(t => t.TodoTitle.Contains("測試")),
            "應包含標題含有「測試」的 Todo"
        );
    }

    /// <summary>
    /// 測試使用完成狀態篩選條件查詢，應返回符合條件的 Todo
    /// </summary>
    [TestMethod]
    public async Task GetTodoByQueryString_WithIsCompleteFilter_ReturnsFilteredTodos()
    {
        // Arrange: 新增兩個 Todo，並將其中一個標記為完成
        var insertRequest1 = new InsertTodoRequest { TodoTitle = "待辦1", TodoContent = "內容1" };
        var insertRequest2 = new InsertTodoRequest { TodoTitle = "待辦2", TodoContent = "內容2" };

        var insertResponse1 = await _client.PostAsJsonAsync("/api/todo/todo", insertRequest1);
        var insertResponse2 = await _client.PostAsJsonAsync("/api/todo/todo", insertRequest2);

        var insertApiResponse1 = await insertResponse1.Content.ReadFromJsonAsync<
            APIResponse<InsertTodoResponse>
        >();
        var insertApiResponse2 = await insertResponse2.Content.ReadFromJsonAsync<
            APIResponse<InsertTodoResponse>
        >();

        // 將第二個 Todo 標記為完成
        var updateRequest = new UpdateTodoByIdRequest
        {
            TodoTitle = "待辦2",
            TodoContent = "內容2",
            IsComplete = "Y",
        };
        await _client.PutAsJsonAsync(
            $"/api/todo/todo/{insertApiResponse2!.Data!.TodoId}",
            updateRequest
        );

        // Act: 使用完成狀態篩選查詢
        var getResponse = await _client.GetAsync("/api/todo/todo?IsComplete=Y");

        // Assert
        getResponse.EnsureSuccessStatusCode();

        var getApiResponse = await getResponse.Content.ReadFromJsonAsync<
            APIResponse<List<GetTodoByQueryStringResponse>>
        >();

        Assert.IsNotNull(getApiResponse);
        Assert.AreEqual(Code.成功, getApiResponse.Code);
        Assert.IsNotNull(getApiResponse.Data);
        Assert.IsTrue(getApiResponse.Data.Count >= 1);
        Assert.IsTrue(getApiResponse.Data.All(t => t.IsComplete == "Y"), "所有結果應為已完成狀態");
    }

    /// <summary>
    /// 測試使用多個篩選條件查詢，應返回符合所有條件的 Todo
    /// </summary>
    [TestMethod]
    public async Task GetTodoByQueryString_WithMultipleFilters_ReturnsFilteredTodos()
    {
        // Arrange: 新增三個 Todo
        var insertRequest1 = new InsertTodoRequest
        {
            TodoTitle = "測試待辦",
            TodoContent = "內容1",
        };
        var insertRequest2 = new InsertTodoRequest
        {
            TodoTitle = "測試待辦2",
            TodoContent = "內容2",
        };
        var insertRequest3 = new InsertTodoRequest
        {
            TodoTitle = "其他待辦",
            TodoContent = "內容3",
        };

        await _client.PostAsJsonAsync("/api/todo/todo", insertRequest1);
        var insertResponse2 = await _client.PostAsJsonAsync("/api/todo/todo", insertRequest2);
        await _client.PostAsJsonAsync("/api/todo/todo", insertRequest3);

        // 將第二個 Todo 標記為完成
        var insertApiResponse2 = await insertResponse2.Content.ReadFromJsonAsync<
            APIResponse<InsertTodoResponse>
        >();
        var updateRequest = new UpdateTodoByIdRequest
        {
            TodoTitle = "測試待辦2",
            TodoContent = "內容2",
            IsComplete = "Y",
        };
        await _client.PutAsJsonAsync(
            $"/api/todo/todo/{insertApiResponse2!.Data!.TodoId}",
            updateRequest
        );

        // Act: 使用多個篩選條件查詢（標題包含「測試」且未完成）
        var getResponse = await _client.GetAsync("/api/todo/todo?TodoTitle=測試&IsComplete=N");

        // Assert
        getResponse.EnsureSuccessStatusCode();

        var getApiResponse = await getResponse.Content.ReadFromJsonAsync<
            APIResponse<List<GetTodoByQueryStringResponse>>
        >();

        Assert.IsNotNull(getApiResponse);
        Assert.AreEqual(Code.成功, getApiResponse.Code);
        Assert.IsNotNull(getApiResponse.Data);
        Assert.IsTrue(getApiResponse.Data.Count >= 1);
        Assert.IsTrue(
            getApiResponse.Data.All(t => t.TodoTitle.Contains("測試") && t.IsComplete == "N"),
            "所有結果應標題包含「測試」且未完成"
        );
    }
}
