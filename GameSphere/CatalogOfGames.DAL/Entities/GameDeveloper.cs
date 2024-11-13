namespace CatalogOfGames.DAL.Entities;

public class GameDeveloper
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public Guid DeveloperId { get; set; }
    public Developer Developer { get; set; } = null!;
}