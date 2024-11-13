using CatalogOfGames.DAL.Enums;

namespace CatalogOfGames.BAL.DTOs;

public  class GetGamesPaginationDTO
{
    public GameSorter GameSorter { get; set; }
    public string SearchText { get; set; } = string.Empty;
    public int? Page {  get; set; }
    public int PageSize { get; set; }
    public string? CategoryName { get; set; }
    public string? UserId { get; set; }
}