namespace CatalogOfGames.DAL.Entities;

public class Platform
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<GamePlatform>? GamePlatforms { get; set; }
}