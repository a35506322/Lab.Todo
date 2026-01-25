namespace TodoAPI.Modules.Auth.User.Login;

public class LoginRequest
{
    [Required]
    [Display(Name = "帳號")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "密碼")]
    public string Password { get; set; } = string.Empty;
}
