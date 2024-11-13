namespace CatalogOfGames.DAL.Entities;

public class Store
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string IconDirectory { get; set; } = null!;

    public ICollection<GameStore> GameStores { get; set; } = null!;
}