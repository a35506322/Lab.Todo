using System.Net.Http.Headers;
using TodoAPI.IntegrationTest.Helpers;
using TodoAPI.Modules.Todo.GetTodoById;
using TodoAPI.Modules.Todo.InsertTodo;

namespace TodoAPI.IntegrationTest.Tests.Todo;

/// <summary>
/// 新增 Todo 端點整合測試
/// </summary>
[TestClass]
public class InsertTodoEndpointTests
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
    /// 測試新增有效的 Todo，應返回成功
    /// </summary>
    [TestMethod]
    public async Task InsertTodo_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new InsertTodoRequest { TodoTitle = "測試待辦", TodoContent = "測試內容" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todo/todo", request);

        // Assert
        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content.ReadFromJsonAsync<
            APIResponse<InsertTodoResponse>
        >();

        Assert.IsNotNull(apiResponse);
        Assert.AreEqual(Code.成功, apiResponse.Code);
        Assert.AreEqual("新增成功", apiResponse.Message);
        Assert.IsNotNull(apiResponse.Data);
        Assert.IsTrue(apiResponse.Data.TodoId > 0);
        Assert.AreEqual(request.TodoTitle, apiResponse.Data.TodoTitle);
        Assert.AreEqual(request.TodoContent, apiResponse.Data.TodoContent);
        Assert.AreEqual("N", apiResponse.Data.IsComplete);

        // 驗證資料已儲存到資料庫：查詢剛新增的 Todo
        var getResponse = await _client.GetAsync($"/api/todo/todo/{apiResponse.Data.TodoId}");
        getResponse.EnsureSuccessStatusCode();

        var getApiResponse = await getResponse.Content.ReadFromJsonAsync<
            APIResponse<GetTodoByIdResponse>
        >();

        Assert.IsNotNull(getApiResponse);
        Assert.AreEqual(Code.成功, getApiResponse.Code);
        Assert.IsNotNull(getApiResponse.Data);
        Assert.AreEqual(request.TodoTitle, getApiResponse.Data.TodoTitle);
        Assert.AreEqual(request.TodoContent, getApiResponse.Data.TodoContent);
        Assert.AreEqual("N", getApiResponse.Data.IsComplete);
    }
}
