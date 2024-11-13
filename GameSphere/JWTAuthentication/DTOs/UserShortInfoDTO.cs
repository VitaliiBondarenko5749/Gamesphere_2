namespace JWTAuthentication.DTOs;

public class UserShortInfoDTO
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string? Role { get; set; }
}