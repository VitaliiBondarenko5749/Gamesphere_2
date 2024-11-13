namespace CatalogOfGames.DAL.Entities;

public class GameImage
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public string ImageDirectory { get; set; } = null!;
}