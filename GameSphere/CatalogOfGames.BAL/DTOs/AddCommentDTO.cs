namespace CatalogOfGames.BAL.DTOs;

public class AddCommentDTO
{
    public string UserId { get; set; } = default!;
    public Guid GameId { get; set; }
    public string Content { get; set; } = default!;
}