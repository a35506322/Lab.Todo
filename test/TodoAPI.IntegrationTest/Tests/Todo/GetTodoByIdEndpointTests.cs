using System.Net;
using System.Net.Http.Headers;
using TodoAPI.IntegrationTest.Helpers;
using TodoAPI.Modules.Todo.GetTodoById;
using TodoAPI.Modules.Todo.InsertTodo;

namespace TodoAPI.IntegrationTest.Tests.Todo;

/// <summary>
/// 依 ID 取得 Todo 端點整合測試
/// </summary>
[TestClass]
public class GetTodoByIdEndpointTests
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
    /// 測試查詢有效的 Todo ID，應返回成功
    /// </summary>
    [TestMethod]
    public async Task GetTodoById_ValidId_ReturnsSuccess()
    {
        // Arrange: 先新增一個 Todo
        var insertRequest = new InsertTodoRequest
        {
            TodoTitle = "測試待辦",
            TodoContent = "測試內容",
        };

        var insertResponse = await _client.PostAsJsonAsync("/api/todo/todo", insertRequest);
        insertResponse.EnsureSuccessStatusCode();

        var insertApiResponse = await insertResponse.Content.ReadFromJsonAsync<
            APIResponse<InsertTodoResponse>
        >();
        var todoId = insertApiResponse!.Data!.TodoId;

        // Act: 查詢 Todo
        var getResponse = await _client.GetAsync($"/api/todo/todo/{todoId}");

        // Assert
        getResponse.EnsureSuccessStatusCode();

        var getApiResponse = await getResponse.Content.ReadFromJsonAsync<
            APIResponse<GetTodoByIdResponse>
        >();

        Assert.IsNotNull(getApiResponse);
        Assert.AreEqual(Code.成功, getApiResponse.Code);
        Assert.AreEqual("查詢成功", getApiResponse.Message);
        Assert.IsNotNull(getApiResponse.Data);
        Assert.AreEqual(todoId, getApiResponse.Data.TodoId);
        Assert.AreEqual(insertRequest.TodoTitle, getApiResponse.Data.TodoTitle);
        Assert.AreEqual(insertRequest.TodoContent, getApiResponse.Data.TodoContent);
        Assert.AreEqual("N", getApiResponse.Data.IsComplete);
    }

    /// <summary>
    /// 測試查詢無效的 Todo ID，應返回商業邏輯錯誤
    /// </summary>
    [TestMethod]
    public async Task GetTodoById_InvalidId_ReturnsBusinessLogicError()
    {
        // Arrange
        var invalidId = 999L;

        // Act
        var getResponse = await _client.GetAsync($"/api/todo/todo/{invalidId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, getResponse.StatusCode);

        var getApiResponse = await getResponse.Content.ReadFromJsonAsync<
            APIResponse<GetTodoByIdResponse>
        >();

        Assert.IsNotNull(getApiResponse);
        Assert.AreEqual(Code.商業邏輯錯誤, getApiResponse.Code);
        Assert.AreEqual("找不到指定的待辦事項", getApiResponse.Message);
    }
}
