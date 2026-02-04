using System.Net.Http.Json;
using TodoAPI.Modules.Auth.User.Login;

namespace TodoAPI.IntegrationTest.Helpers;

/// <summary>
/// 測試認證輔助類別
/// </summary>
public static class TestAuthHelper
{
    /// <summary>
    /// 取得測試用 JWT Token
    /// </summary>
    /// <param name="client">HttpClient</param>
    /// <param name="userId">使用者 ID</param>
    /// <param name="password">密碼</param>
    /// <returns>JWT Token</returns>
    public static async Task<string> GetTokenAsync(
        HttpClient client,
        string userId = "admin",
        string password = "admin123"
    )
    {
        var loginRequest = new LoginRequest { UserId = userId, Password = password };

        var response = await client.PostAsJsonAsync("/api/user/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<LoginResponse>>();

        if (apiResponse?.Data?.Token == null)
        {
            throw new InvalidOperationException("無法取得 Token");
        }

        return apiResponse.Data.Token;
    }
}
