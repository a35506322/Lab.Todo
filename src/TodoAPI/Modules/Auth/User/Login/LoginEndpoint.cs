namespace TodoAPI.Modules.Auth.User.Login;

public class LoginEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/login", Handle).WithTags("Login");

    private static async Task<IResult> Handle(
        LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        return TypedResults.Ok(new LoginResponse());
    }
}
