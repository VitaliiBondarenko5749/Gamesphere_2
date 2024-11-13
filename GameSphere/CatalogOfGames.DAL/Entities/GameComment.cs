namespace CatalogOfGames.DAL.Entities;

public class GameComment
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;

    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ICollection<LikedComment>? LikedComments { get; set; }
}