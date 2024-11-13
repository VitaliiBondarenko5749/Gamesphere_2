namespace CatalogOfGames.BAL.DTOs;

public class UpdatePublisherInGameDTO
{
    public Guid PublisherId { get; set; }
    public Guid GameId { get; set; }
}