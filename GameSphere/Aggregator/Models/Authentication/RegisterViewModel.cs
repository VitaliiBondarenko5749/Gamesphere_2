namespace Aggregator.Models.Authentication;

public class RegisterViewModel
{
    public IFormFile? Avatar { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
