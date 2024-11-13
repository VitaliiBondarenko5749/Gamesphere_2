namespace CatalogOfGames.BAL.DTOs;

public class PlatformToGameDTO
{
    public Guid PlatformId { get; set; }
    public string GameName { get; set; } = default!;
}