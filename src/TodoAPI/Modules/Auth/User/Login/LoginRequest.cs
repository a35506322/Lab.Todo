namespace TodoAPI.Modules.Auth.User.Login;

public class LoginRequest
{
    /// <summary>
    /// 帳號
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
