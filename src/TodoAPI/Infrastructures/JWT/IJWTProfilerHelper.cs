namespace TodoAPI.Infrastructures.JWT;

public interface IJWTProfilerHelper
{
    string UserId { get; }

    string[] Roles { get; }

    string UserName { get; }
}
