namespace CatalogOfGames.DAL.Entities;

public class GameVideoLink
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public string Link { get; set; } = null!;
}