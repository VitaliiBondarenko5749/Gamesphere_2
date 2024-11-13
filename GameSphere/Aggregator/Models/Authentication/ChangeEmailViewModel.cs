namespace Aggregator.Models.Authentication;

public class ChangeEmailViewModel
{
    public string UserId { get; set; } = default!;
    public string NewEmail { get; set; } = default!;
}