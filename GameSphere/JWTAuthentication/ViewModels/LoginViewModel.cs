namespace JWTAuthentication.ViewModels;

public class LoginViewModel
{
    public string UsernameOrEmailInput { get; set; } = default!;
    public string Password { get; set; } = default!;
}