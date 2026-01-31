namespace TodoAPI.Modules.Auth.User.Login;

public class LoginEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/login", Handler).WithTags("Login");

    private static async Task<IResult> Handler(
        LoginRequest request,
        LabContext context,
        CancellationToken cancellationToken
    )
    {
        var user = await context
            .User.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == request.UserId && x.Password == request.Password,
                cancellationToken
            );

        if (user is null)
        {
            return APIResponseHelper.BusinessLogicError<LoginResponse>(message: "帳號或密碼不正確");
        }

        return APIResponseHelper.Ok(message: "登入成功", data: new LoginResponse());
    }
}
