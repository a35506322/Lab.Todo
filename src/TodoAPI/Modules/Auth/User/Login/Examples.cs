namespace TodoAPI.Modules.Auth.User.Login;

public class LoginReqEx_Admin : IExampleProvider
{
    public object GetExample() => new LoginRequest { UserId = "admin", Password = "123456" };
}

public class LoginReqEx_Demo : IExampleProvider
{
    public object GetExample() => new LoginRequest { UserId = "demo", Password = "123456" };
}

public class LoginResEx_Ok_LoginSuccess : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<LoginResponse>(
            Code.成功,
            "登入成功",
            new LoginResponse { Token = "1234567890", ExpiresIn = 60 }
        );
}

public class LoginResEx_422_AccountOrPasswordIncorrect : IExampleProvider
{
    public object GetExample() =>
        new APIResponse<LoginResponse>(Code.商業邏輯錯誤, "帳號或密碼不正確", default);
}
