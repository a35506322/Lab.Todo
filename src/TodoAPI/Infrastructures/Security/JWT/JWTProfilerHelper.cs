namespace TodoAPI.Infrastructures.Security.JWT;

public class JWTProfilerHelper(IHttpContextAccessor httpContextAccessor) : IJWTProfilerHelper
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public string UserId =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? string.Empty;

    /// <summary>
    /// 使用者角色
    /// </summary>
    public string[] Roles =>
        httpContextAccessor
            .HttpContext?.User?.FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .ToArray() ?? Array.Empty<string>();

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string UserName =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Name)
        ?? string.Empty;
}
