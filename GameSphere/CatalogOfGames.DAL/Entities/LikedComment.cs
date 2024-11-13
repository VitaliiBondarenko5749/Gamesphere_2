namespace CatalogOfGames.DAL.Entities;

public class LikedComment
{
    public Guid Id { get; set; }

    public Guid CommentId { get; set; }
    public GameComment Comment { get; set; } = null!;

    public string UserId { get; set; } = null!;
}