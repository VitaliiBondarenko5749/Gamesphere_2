namespace CatalogOfGames.DAL.Entities;

public class FavoriteGame
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public string UserId { get; set; } = null!;
}