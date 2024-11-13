namespace JWTAuthentication.ViewModels;

public class ChangeEmailViewModel
{
    public string UserId { get; set; } = default!;
    public string NewEmail { get; set; } = default!;
    public string? Code { get; set; }
}