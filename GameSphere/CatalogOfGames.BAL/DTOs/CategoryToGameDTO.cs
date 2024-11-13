namespace CatalogOfGames.BAL.DTOs;

public class CategoryToGameDTO
{
    public Guid CategoryId { get; set; }
    public string GameName { get; set; } = default!;
}