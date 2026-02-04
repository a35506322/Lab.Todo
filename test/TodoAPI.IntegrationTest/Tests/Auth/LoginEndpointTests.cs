using System.Net;
using TodoAPI.IntegrationTest.Helpers;
using TodoAPI.Modules.Auth.User.Login;

namespace TodoAPI.IntegrationTest.Tests.Auth;

/// <summary>
/// 登入端點整合測試
/// </summary>
[TestClass]
public class LoginEndpointTests
{
    private static CustomWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

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
    public void Setup()
    {
        _client = _factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
    }

    /// <summary>
    /// 測試使用有效的憑證登入，應返回成功並取得 Token
    /// </summary>
    [TestMethod]
    public async Task Login_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var request = new LoginRequest { UserId = "admin", Password = "admin123" };
        // Act
        var response = await _client.PostAsJsonAsync("/api/user/login", request);

        // Assert
        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<LoginResponse>>();

        Assert.IsNotNull(apiResponse);
        Assert.AreEqual(Code.成功, apiResponse.Code);
        Assert.AreEqual("登入成功", apiResponse.Message);
        Assert.IsNotNull(apiResponse.Data);
        Assert.IsFalse(string.IsNullOrEmpty(apiResponse.Data.Token), "Token 不應為空");
        Assert.IsTrue(apiResponse.Data.ExpiresIn > 0, "ExpiresIn 應大於 0");
    }

    /// <summary>
    /// 測試使用無效的憑證登入，應返回商業邏輯錯誤
    /// </summary>
    [TestMethod]
    public async Task Login_InvalidCredentials_ReturnsBusinessLogicError()
    {
        // Arrange
        var request = new LoginRequest { UserId = "invalid", Password = "wrong" };
        // Act
        var response = await _client.PostAsJsonAsync("/api/user/login", request);

        // Assert
        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<LoginResponse>>();

        Assert.IsNotNull(apiResponse);
        Assert.AreEqual(Code.商業邏輯錯誤, apiResponse.Code);
        Assert.AreEqual("帳號或密碼不正確", apiResponse.Message);
    }

    /// <summary>
    /// 測試使用錯誤的密碼登入，應返回商業邏輯錯誤
    /// </summary>
    [TestMethod]
    public async Task Login_WrongPassword_ReturnsBusinessLogicError()
    {
        // Arrange
        var request = new LoginRequest { UserId = "admin", Password = "wrongpassword" };
        // Act
        var response = await _client.PostAsJsonAsync("/api/user/login", request);

        // Assert
        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<LoginResponse>>();

        Assert.IsNotNull(apiResponse);
        Assert.AreEqual(Code.商業邏輯錯誤, apiResponse.Code);
        Assert.AreEqual("帳號或密碼不正確", apiResponse.Message);
    }

    /// <summary>
    /// 測試使用取得的 Token 進行後續 API 呼叫，應成功通過認證
    /// </summary>
    [TestMethod]
    public async Task Login_ValidToken_CanAccessProtectedEndpoint()
    {
        // Arrange: 先登入取得 Token
        var loginRequest = new LoginRequest { UserId = "admin", Password = "admin123" };
        var loginHttpResponse = await _client.PostAsJsonAsync("/api/user/login", loginRequest);
        loginHttpResponse.EnsureSuccessStatusCode();

        var loginApiResponse = await loginHttpResponse.Content.ReadFromJsonAsync<
            APIResponse<LoginResponse>
        >();
        var token = loginApiResponse!.Data!.Token;

        // Act: 使用 Token 呼叫受保護的端點
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 嘗試查詢 Todo 列表（需要認證）
        var getResponse = await _client.GetAsync("/api/todo/todo");

        // Assert: 應成功通過認證（可能返回空列表或成功）
        Assert.AreNotEqual(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        Assert.AreNotEqual(HttpStatusCode.Forbidden, getResponse.StatusCode);
    }
}
