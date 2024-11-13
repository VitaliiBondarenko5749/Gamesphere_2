namespace JWTAuthentication.Data.Models;

public class ConfirmEmailModel
{
    public string UserId { get; set; } = default!;
    public string Code { get; set; } = default!;
}