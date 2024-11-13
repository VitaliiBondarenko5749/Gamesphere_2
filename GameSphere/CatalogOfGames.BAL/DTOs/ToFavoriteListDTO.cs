namespace CatalogOfGames.BAL.DTOs;

public class ToFavoriteListDTO
{
    public Guid GameId { get; set; }
    public string UserId { get; set; } = default!;
}