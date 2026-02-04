namespace TodoAPI.Infrastructures.Security.JWT;

public interface IJWTHelper
{
    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    /// <param name="userName">使用者帳號</param>
    /// <param name="userId">使用者名稱</param>
    /// <param name="roles">使用者角色</param>
    /// <returns>JWT Token</returns>
    public string GenerateToken(
        string userId,
        string? userName = null,
        IList<string>? roles = null
    );

    /// <summary>
    /// 獲取 Token 時效 (單位: 分鐘)
    /// </summary>
    /// <returns>Token 時效 (單位: 分鐘)</returns>
    public int GetExpiresIn();
}
