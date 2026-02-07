---
name: mstest-integration-test
description: 適用於 MSTest 整合測試最佳實踐模式，裡面包含 MSTest 整合測試、NSubstitute Mock、Entity Framework Core InMemory 等
---

# MSTest 整合測試規範

適用於 MSTest 整合測試最佳實踐模式，裡面包含`MSTest 整合測試`、`NSubstitute Mock`、`Entity Framework Core InMemory`等

## 使用場景

- 新增/修改 API 端點
- 測試完整流程

## 測試工具

- MSTest
- NSubstitute (Mock 工具)
- Entity Framework Core InMemory (In-Memory 資料庫)
- UnitTesting.Assert (斷言工具)

## MSTest 整合測試

主要測試引用專案的 API 端點

### 資料夾結構

```
Tests # 整合測試主目錄
├── Auth # 認證模組整合測試
│   └── LoginEndpointTests.cs # 登入端點整合測試
└── Todo # Todo 模組整合測試
    ├── DeleteTodoByIdEndpointTests.cs # 依 ID 刪除 Todo 端點整合測試
    ├── GetTodoByIdEndpointTests.cs # 依 ID 取得 Todo 端點整合測試
    ├── GetTodoByQueryStringEndpointTests.cs # 依查詢字串取得 Todo 端點整合測試
    ├── InsertTodoEndpointTests.cs # 新增 Todo 端點整合測試
    ├── TodoCrudFlowTests.cs # Todo CRUD 流程整合測試
    └── UpdateTodoByIdEndpointTests.cs # 依 ID 更新 Todo 端點整合測試
```

### 命名規範

| 類型                 | 命名規範              |
| -------------------- | --------------------- |
| 測試單個端點檔名     | [XXX]EndpointTests.cs |
| 測試某個流程方法檔名 | [XXX]FlowTests.cs     |

### 初始化設定

**引用 CustomWebApplicationFactory**

```csharp
using TodoAPI.IntegrationTest.Helpers;

private static CustomWebApplicationFactory<Program> \_factory = null!;

[ClassInitialize]
public static void ClassInitialize(TestContext _)
{
    _factory = new CustomWebApplicationFactory<Program>();
}
```

### Flow Test 範例

```csharp
/// <summary>
/// Todo CRUD 正向流程整合測試
/// </summary>
[TestClass]
public class TodoCrudFlowTests
{
private static CustomWebApplicationFactory<Program> \_factory = null!;
private HttpClient \_client = null!;
private string \_token = null!;

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

```

### Endpoint Test 範例

```csharp
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

```
