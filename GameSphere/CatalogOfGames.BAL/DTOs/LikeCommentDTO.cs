namespace CatalogOfGames.BAL.DTOs;

public class LikeCommentDTO
{
    public Guid CommentId { get; set; }
    public string UserId { get; set; } = default!;
}