namespace Aggregator.Models.CatalogOfGames;

public class CommentInfoDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string CreatedAt { get; set; } = default!;
    public string UserId { get; set; } = default!;
}