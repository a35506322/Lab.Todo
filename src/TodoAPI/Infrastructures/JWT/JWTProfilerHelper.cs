namespace TodoAPI.Infrastructures.JWT;

public class JWTProfilerHelper(IHttpContextAccessor httpContextAccessor) : IJWTProfilerHelper
{
    public string UserId =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? string.Empty;

    public string[] Roles =>
        httpContextAccessor
            .HttpContext?.User?.FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .ToArray() ?? Array.Empty<string>();
    public string UserName =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Name)
        ?? string.Empty;
}
