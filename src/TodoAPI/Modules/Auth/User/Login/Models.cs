namespace TodoAPI.Modules.Auth.User.Login;

public class LoginRequest
{
    /// <summary>
    /// 帳號
    /// </summary>
    [Required]
    [Display(Name = "帳號")]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required]
    [Display(Name = "密碼")]
    public string Password { get; set; } = null!;
};

public class LoginResponse
{
    /// <summary>
    /// 登入 Token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token 時效 (單位: 分鐘)
    /// </summary>
    public int ExpiresIn { get; set; } = 0;
}
