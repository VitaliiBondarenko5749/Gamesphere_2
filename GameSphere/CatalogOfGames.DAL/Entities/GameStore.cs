namespace CatalogOfGames.DAL.Entities;

public class GameStore
{
    public Guid Id { get; set; }

    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;

    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public string Link { get; set; } = null!;
}