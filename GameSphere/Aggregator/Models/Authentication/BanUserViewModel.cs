namespace Aggregator.Models.Authentication;

public class BanUserViewModel
{
    public string UserId { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public string BannedBy { get; set; } = default!;
}