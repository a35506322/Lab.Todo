namespace TodoAPI.Infrastructures.JWT;

public interface IJWTProfilerHelper
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// 使用者角色
    /// </summary>
    string[] Roles { get; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    string UserName { get; }
}
