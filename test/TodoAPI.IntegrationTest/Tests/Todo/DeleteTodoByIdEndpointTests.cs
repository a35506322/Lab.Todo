using System.Net;
using System.Net.Http.Headers;
using TodoAPI.IntegrationTest.Helpers;
using TodoAPI.Modules.Todo.DeleteTodoById;
using TodoAPI.Modules.Todo.GetTodoById;
using TodoAPI.Modules.Todo.InsertTodo;

namespace TodoAPI.IntegrationTest.Tests.Todo;

/// <summary>
/// 依 ID 刪除 Todo 端點整合測試
/// </summary>
[TestClass]
public class DeleteTodoByIdEndpointTests
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
    /// 測試刪除有效的 Todo ID，應返回成功
    /// </summary>
    [TestMethod]
    public async Task DeleteTodoById_ValidId_ReturnsSuccess()
    {
        // Arrange: 先新增一個 Todo
        var insertRequest = new InsertTodoRequest
        {
            TodoTitle = "待刪除的待辦事項",
            TodoContent = "這是待刪除的內容",
        };

        var insertResponse = await _client.PostAsJsonAsync("/api/todo/todo", insertRequest);
        insertResponse.EnsureSuccessStatusCode();

        var insertApiResponse = await insertResponse.Content.ReadFromJsonAsync<
            APIResponse<InsertTodoResponse>
        >();
        var todoId = insertApiResponse!.Data!.TodoId;

        // Act: 刪除 Todo
        var deleteResponse = await _client.DeleteAsync($"/api/todo/todo/{todoId}");

        // Assert
        deleteResponse.EnsureSuccessStatusCode();

        var deleteApiResponse = await deleteResponse.Content.ReadFromJsonAsync<
            APIResponse<DeleteTodoByIdResponse>
        >();

        Assert.IsNotNull(deleteApiResponse);
        Assert.AreEqual(Code.成功, deleteApiResponse.Code);
        Assert.AreEqual("刪除成功", deleteApiResponse.Message);
        Assert.IsNotNull(deleteApiResponse.Data);
        Assert.AreEqual(todoId, deleteApiResponse.Data.TodoId);

        // 驗證資料已刪除：查詢應返回 422
        var getAfterDeleteResponse = await _client.GetAsync($"/api/todo/todo/{todoId}");
        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, getAfterDeleteResponse.StatusCode);

        var getAfterDeleteApiResponse = await getAfterDeleteResponse.Content.ReadFromJsonAsync<
            APIResponse<GetTodoByIdResponse>
        >();

        Assert.IsNotNull(getAfterDeleteApiResponse);
        Assert.AreEqual(Code.商業邏輯錯誤, getAfterDeleteApiResponse.Code);
        Assert.AreEqual("找不到指定的待辦事項", getAfterDeleteApiResponse.Message);
    }

    /// <summary>
    /// 測試刪除無效的 Todo ID，應返回商業邏輯錯誤
    /// </summary>
    [TestMethod]
    public async Task DeleteTodoById_InvalidId_ReturnsBusinessLogicError()
    {
        // Arrange
        var invalidId = 999L;

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/todo/todo/{invalidId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, deleteResponse.StatusCode);

        var deleteApiResponse = await deleteResponse.Content.ReadFromJsonAsync<
            APIResponse<DeleteTodoByIdResponse>
        >();

        Assert.IsNotNull(deleteApiResponse);
        Assert.AreEqual(Code.商業邏輯錯誤, deleteApiResponse.Code);
        Assert.AreEqual("找不到指定的待辦事項", deleteApiResponse.Message);
    }
}
