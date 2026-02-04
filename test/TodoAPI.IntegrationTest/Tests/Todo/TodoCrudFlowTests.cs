using System.Net;
using System.Net.Http.Headers;
using TodoAPI.IntegrationTest.Helpers;
using TodoAPI.Modules.Todo.DeleteTodoById;
using TodoAPI.Modules.Todo.GetTodoById;
using TodoAPI.Modules.Todo.InsertTodo;
using TodoAPI.Modules.Todo.UpdateTodoById;

namespace TodoAPI.IntegrationTest.Tests.Todo;

/// <summary>
/// Todo CRUD 正向流程整合測試
/// </summary>
[TestClass]
public class TodoCrudFlowTests
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
    /// 測試完整 CRUD 流程：登入 → 新增 → 查詢 → 更新 → 刪除
    /// </summary>
    [TestMethod]
    public async Task TodoCrudFlow_CompleteFlow_ReturnsSuccess()
    {
        // Arrange
        var insertRequest = new InsertTodoRequest
        {
            TodoTitle = "整合測試待辦事項",
            TodoContent = "這是整合測試的待辦事項內容",
        };

        // Act 1: 新增待辦事項
        var todoId = await InsertTodoAsync(insertRequest);

        // Act 2: 查詢待辦事項
        await GetTodoByIdAsync(todoId, "整合測試待辦事項", "這是整合測試的待辦事項內容", "N");

        // Act 3: 更新待辦事項
        await UpdateTodoByIdAsync(todoId);

        // Act 4: 刪除待辦事項
        await DeleteTodoByIdAsync(todoId);

        // Act 5: 驗證刪除後查詢應返回 422
        await VerifyTodoDeletedAsync(todoId);
    }

    /// <summary>
    /// 新增待辦事項
    /// </summary>
    private async Task<long> InsertTodoAsync(InsertTodoRequest request)
    {
        var insertResponse = await _client.PostAsJsonAsync("/api/todo/todo", request);
        insertResponse.EnsureSuccessStatusCode();

        var insertApiResponse = await insertResponse.Content.ReadFromJsonAsync<
            APIResponse<InsertTodoResponse>
        >();

        Assert.IsNotNull(insertApiResponse);
        Assert.AreEqual(Code.成功, insertApiResponse.Code);
        Assert.IsNotNull(insertApiResponse.Data);
        Assert.IsTrue(insertApiResponse.Data.TodoId > 0);
        Assert.AreEqual(request.TodoTitle, insertApiResponse.Data.TodoTitle);
        Assert.AreEqual(request.TodoContent, insertApiResponse.Data.TodoContent);
        Assert.AreEqual("N", insertApiResponse.Data.IsComplete);

        return insertApiResponse.Data.TodoId;
    }

    /// <summary>
    /// 查詢待辦事項
    /// </summary>
    private async Task GetTodoByIdAsync(
        long todoId,
        string expectedTitle,
        string expectedContent,
        string expectedIsComplete
    )
    {
        var getResponse = await _client.GetAsync($"/api/todo/todo/{todoId}");
        getResponse.EnsureSuccessStatusCode();

        var getApiResponse = await getResponse.Content.ReadFromJsonAsync<
            APIResponse<GetTodoByIdResponse>
        >();

        Assert.IsNotNull(getApiResponse);
        Assert.AreEqual(Code.成功, getApiResponse.Code);
        Assert.IsNotNull(getApiResponse.Data);
        Assert.AreEqual(todoId, getApiResponse.Data.TodoId);
        Assert.AreEqual(expectedTitle, getApiResponse.Data.TodoTitle);
        Assert.AreEqual(expectedContent, getApiResponse.Data.TodoContent);
        Assert.AreEqual(expectedIsComplete, getApiResponse.Data.IsComplete);
    }

    /// <summary>
    /// 更新待辦事項
    /// </summary>
    private async Task UpdateTodoByIdAsync(long todoId)
    {
        var updateRequest = new UpdateTodoByIdRequest
        {
            TodoTitle = "更新後的待辦事項標題",
            TodoContent = "更新後的待辦事項內容",
            IsComplete = "Y",
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/todo/todo/{todoId}",
            updateRequest
        );
        updateResponse.EnsureSuccessStatusCode();

        var updateApiResponse = await updateResponse.Content.ReadFromJsonAsync<
            APIResponse<UpdateTodoByIdResponse>
        >();

        Assert.IsNotNull(updateApiResponse);
        Assert.AreEqual(Code.成功, updateApiResponse.Code);
        Assert.IsNotNull(updateApiResponse.Data);
        Assert.AreEqual(todoId, updateApiResponse.Data.TodoId);
        Assert.AreEqual("更新後的待辦事項標題", updateApiResponse.Data.TodoTitle);
        Assert.AreEqual("更新後的待辦事項內容", updateApiResponse.Data.TodoContent);
        Assert.AreEqual("Y", updateApiResponse.Data.IsComplete);
        Assert.IsNotNull(updateApiResponse.Data.CompleteTime);
    }

    /// <summary>
    /// 刪除待辦事項
    /// </summary>
    private async Task DeleteTodoByIdAsync(long todoId)
    {
        var deleteResponse = await _client.DeleteAsync($"/api/todo/todo/{todoId}");
        deleteResponse.EnsureSuccessStatusCode();

        var deleteApiResponse = await deleteResponse.Content.ReadFromJsonAsync<
            APIResponse<DeleteTodoByIdResponse>
        >();

        Assert.IsNotNull(deleteApiResponse);
        Assert.AreEqual(Code.成功, deleteApiResponse.Code);
        Assert.IsNotNull(deleteApiResponse.Data);
        Assert.AreEqual(todoId, deleteApiResponse.Data.TodoId);
    }

    /// <summary>
    /// 驗證刪除後查詢應返回 422
    /// </summary>
    private async Task VerifyTodoDeletedAsync(long todoId)
    {
        var getAfterDeleteResponse = await _client.GetAsync($"/api/todo/todo/{todoId}");
        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, getAfterDeleteResponse.StatusCode);

        var getAfterDeleteApiResponse = await getAfterDeleteResponse.Content.ReadFromJsonAsync<
            APIResponse<GetTodoByIdResponse>
        >();

        Assert.IsNotNull(getAfterDeleteApiResponse);
        Assert.AreEqual(Code.商業邏輯錯誤, getAfterDeleteApiResponse.Code);
    }
}
