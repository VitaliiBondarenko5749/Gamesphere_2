namespace CatalogOfGames.DAL.Entities;

public class Language
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<GameLanguage>? GameLanguages { get; set; }
}