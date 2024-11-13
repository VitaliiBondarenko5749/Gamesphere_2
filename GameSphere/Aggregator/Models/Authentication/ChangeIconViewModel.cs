namespace Aggregator.Models.Authentication;

public class ChangeIconViewModel
{
    public string Id { get; set; } = default!;
    public IFormFile NewAvatar { get; set; } = default!;
}