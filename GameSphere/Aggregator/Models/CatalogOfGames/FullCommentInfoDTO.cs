namespace Aggregator.Models.CatalogOfGames;

public class FullCommentInfoDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string CreatedAt { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string IconDirectory { get; set; } = default!;
}