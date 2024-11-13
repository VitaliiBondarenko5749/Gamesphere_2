namespace Aggregator.Models.Authentication;

public class UserInfoDTO
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string IconDirectory { get; set; } = default!;
}