namespace Aggregator.Models.CloudStorage;

public class AddGameImageViewModel
{
    public IFormFile Image { get; set; } = default!;
    public string GameName { get; set; } = default!;
    public string Directory { get; set; } = default!;
}