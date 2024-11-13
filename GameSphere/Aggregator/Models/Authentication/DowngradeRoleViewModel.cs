namespace Aggregator.Models.Authentication;

public class DowngradeRoleViewModel
{
    public string UserId { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
}