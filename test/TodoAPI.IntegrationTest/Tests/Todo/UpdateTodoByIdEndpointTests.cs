using System.Net;
using System.Net.Http.Headers;
using TodoAPI.IntegrationTest.Helpers;
using TodoAPI.Modules.Todo.GetTodoById;
using TodoAPI.Modules.Todo.InsertTodo;
using TodoAPI.Modules.Todo.UpdateTodoById;

namespace TodoAPI.IntegrationTest.Tests.Todo;

/// <summary>
/// 依 ID 更新 Todo 端點整合測試
/// </summary>
[TestClass]
public class UpdateTodoByIdEndpointTests
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
    /// 測試更新有效的 Todo，應返回成功
    /// </summary>
    [TestMethod]
    public async Task UpdateTodoById_ValidRequest_ReturnsSuccess()
    {
        // Arrange: 先新增一個 Todo
        var insertRequest = new InsertTodoRequest
        {
            TodoTitle = "原始標題",
            TodoContent = "原始內容",
        };

        var insertResponse = await _client.PostAsJsonAsync("/api/todo/todo", insertRequest);
        insertResponse.EnsureSuccessStatusCode();

        var insertApiResponse = await insertResponse.Content.ReadFromJsonAsync<
            APIResponse<InsertTodoResponse>
        >();
        var todoId = insertApiResponse!.Data!.TodoId;

        // Act: 更新 Todo
        var updateRequest = new UpdateTodoByIdRequest
        {
            TodoTitle = "新標題",
            TodoContent = "新內容",
            IsComplete = "N",
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/todo/todo/{todoId}",
            updateRequest
        );

        // Assert
        updateResponse.EnsureSuccessStatusCode();

        var updateApiResponse = await updateResponse.Content.ReadFromJsonAsync<
            APIResponse<UpdateTodoByIdResponse>
        >();

        Assert.IsNotNull(updateApiResponse);
        Assert.AreEqual(Code.成功, updateApiResponse.Code);
        Assert.AreEqual("更新成功", updateApiResponse.Message);
        Assert.IsNotNull(updateApiResponse.Data);
        Assert.AreEqual(todoId, updateApiResponse.Data.TodoId);
        Assert.AreEqual(updateRequest.TodoTitle, updateApiResponse.Data.TodoTitle);
        Assert.AreEqual(updateRequest.TodoContent, updateApiResponse.Data.TodoContent);
        Assert.AreEqual(updateRequest.IsComplete, updateApiResponse.Data.IsComplete);

        // 驗證資料已更新：查詢更新後的 Todo
        var getResponse = await _client.GetAsync($"/api/todo/todo/{todoId}");
        getResponse.EnsureSuccessStatusCode();

        var getApiResponse = await getResponse.Content.ReadFromJsonAsync<
            APIResponse<GetTodoByIdResponse>
        >();

        Assert.IsNotNull(getApiResponse);
        Assert.AreEqual(Code.成功, getApiResponse.Code);
        Assert.IsNotNull(getApiResponse.Data);
        Assert.AreEqual(updateRequest.TodoTitle, getApiResponse.Data.TodoTitle);
        Assert.AreEqual(updateRequest.TodoContent, getApiResponse.Data.TodoContent);
        Assert.AreEqual(updateRequest.IsComplete, getApiResponse.Data.IsComplete);
    }

    /// <summary>
    /// 測試更新無效的 Todo ID，應返回商業邏輯錯誤
    /// </summary>
    [TestMethod]
    public async Task UpdateTodoById_InvalidId_ReturnsBusinessLogicError()
    {
        // Arrange
        var invalidId = 999L;
        var updateRequest = new UpdateTodoByIdRequest
        {
            TodoTitle = "新標題",
            TodoContent = "新內容",
            IsComplete = "N",
        };

        // Act
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/todo/todo/{invalidId}",
            updateRequest
        );

        // Assert
        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, updateResponse.StatusCode);

        var updateApiResponse = await updateResponse.Content.ReadFromJsonAsync<
            APIResponse<UpdateTodoByIdResponse>
        >();

        Assert.IsNotNull(updateApiResponse);
        Assert.AreEqual(Code.商業邏輯錯誤, updateApiResponse.Code);
        Assert.AreEqual("找不到指定的待辦事項", updateApiResponse.Message);
    }

    /// <summary>
    /// 測試將 Todo 標記為完成，應設定完成時間
    /// </summary>
    [TestMethod]
    public async Task UpdateTodoById_UpdateToComplete_UpdatesCompleteTime()
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

        // Act: 將 Todo 標記為完成
        var updateRequest = new UpdateTodoByIdRequest
        {
            TodoTitle = "測試待辦",
            TodoContent = "測試內容",
            IsComplete = "Y",
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/todo/todo/{todoId}",
            updateRequest
        );

        // Assert
        updateResponse.EnsureSuccessStatusCode();

        var updateApiResponse = await updateResponse.Content.ReadFromJsonAsync<
            APIResponse<UpdateTodoByIdResponse>
        >();

        Assert.IsNotNull(updateApiResponse);
        Assert.AreEqual(Code.成功, updateApiResponse.Code);
        Assert.IsNotNull(updateApiResponse.Data);
        Assert.AreEqual("Y", updateApiResponse.Data.IsComplete);
        Assert.IsNotNull(updateApiResponse.Data.CompleteTime, "完成時間應已設定");

        // 驗證資料已更新：查詢更新後的 Todo
        var getResponse = await _client.GetAsync($"/api/todo/todo/{todoId}");
        getResponse.EnsureSuccessStatusCode();

        var getApiResponse = await getResponse.Content.ReadFromJsonAsync<
            APIResponse<GetTodoByIdResponse>
        >();

        Assert.IsNotNull(getApiResponse);
        Assert.AreEqual(Code.成功, getApiResponse.Code);
        Assert.IsNotNull(getApiResponse.Data);
        Assert.AreEqual("Y", getApiResponse.Data.IsComplete);
        Assert.IsNotNull(getApiResponse.Data.CompleteTime, "完成時間應已設定");
    }
}
