namespace CatalogOfGames.DAL.Entities;

public class Publisher
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Game>? Games { get; set; }
}