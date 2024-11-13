namespace JWTAuthentication.ViewModels;

public class RegisterViewModel
{ 
    public string Username { get; set; } =  default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ImageDirectory { get; set; } = string.Empty;
}