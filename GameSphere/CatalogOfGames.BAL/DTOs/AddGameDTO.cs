namespace CatalogOfGames.BAL.DTOs;

public class AddGameDTO
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid PublisherId { get; set; }
    public DateTime ReleaseDate { get; set; }
}