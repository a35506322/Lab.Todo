namespace TodoAPI.Modules.Auth.User.Login;

public class LoginEndpoint : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app.MapPost("/login", Handler);

    [EndpointName("Login")]
    [EndpointSummary("登入")]
    [EndpointDescription("登入成功後回傳 Token")]
    [ProducesResponseType<APIResponse<LoginResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<APIResponse<LoginResponse>>(StatusCodes.Status422UnprocessableEntity)]
    [RequestExample(typeof(LoginReqEx_Admin), "Admin")]
    [RequestExample(typeof(LoginReqEx_Demo), "測試帳號")]
    [ResponseExample(StatusCodes.Status200OK, typeof(LoginResEx_Ok_LoginSuccess), "登入成功")]
    [ResponseExample(
        StatusCodes.Status422UnprocessableEntity,
        typeof(LoginResEx_422_AccountOrPasswordIncorrect),
        "帳號或密碼不正確"
    )]
    private static async Task<IResult> Handler(
        LoginRequest request,
        LabContext context,
        IJWTHelper jwtHelper,
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

        var token = jwtHelper.GenerateToken(user.UserId);
        var expiresIn = jwtHelper.GetExpiresIn();
        return APIResponseHelper.Ok(
            message: "登入成功",
            data: new LoginResponse { Token = token, ExpiresIn = expiresIn }
        );
    }
}
