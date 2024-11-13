namespace CatalogOfGames.BAL.DTOs;

public class LanguageToGameDTO
{
    public Guid LanguageId { get; set; }
    public string GameName { get; set; } = default!;
}