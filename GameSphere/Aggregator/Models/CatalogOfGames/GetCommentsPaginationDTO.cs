namespace Aggregator.Models.CatalogOfGames;

public class GetCommentsPaginationDTO
{
    public Guid GameId { get; set; }
    public int? Page { get; set; }
    public int PageSize { get; set; }
}