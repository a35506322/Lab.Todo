namespace TodoAPI.UnitTest.Common.Helpers;

/// <summary>
/// 資料庫測試輔助類別
/// </summary>
public static class DatabaseTestHelper
{
    /// <summary>
    /// 建立測試用的 User
    /// </summary>
    /// <param name="context">資料庫上下文</param>
    /// <param name="userId">使用者 ID</param>
    /// <param name="password">密碼</param>
    /// <param name="role">角色</param>
    /// <returns>建立的 User 實體</returns>
    public static async Task<User> CreateTestUserAsync(
        LabContext context,
        string userId,
        string password,
        string role
    )
    {
        var user = new User
        {
            UserId = userId,
            Password = password,
            Role = role,
        };
        context.User.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}
