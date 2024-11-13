namespace CatalogOfGames.BAL.DTOs;

public class DeveloperToGameDTO
{
    public Guid DeveloperId { get; set; }
    public string GameName { get; set; } = default!;
}