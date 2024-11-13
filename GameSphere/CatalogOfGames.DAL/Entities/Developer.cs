namespace CatalogOfGames.DAL.Entities;

public class Developer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<GameDeveloper>? GameDevelopers { get; set; }
}