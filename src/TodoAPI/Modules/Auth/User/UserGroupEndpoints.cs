using TodoAPI.Modules.Auth.User.Login;

namespace TodoAPI.Modules.Auth.User;

public static class UserGroupEndpoints
{
    public static void MapUserGroupEndpoints(this IEndpointRouteBuilder app)
    {
        var userEndpoints = app.MapGroup("/user").WithTags("User");
        userEndpoints.MapEndpoint<LoginEndpoint>();
    }
}
