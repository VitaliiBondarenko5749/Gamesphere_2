namespace Aggregator.Models.CloudStorage;

public class DeleteGameImageViewModel
{
    public Guid Id { get; set; }
    public string Directory { get; set; } = default!;
}