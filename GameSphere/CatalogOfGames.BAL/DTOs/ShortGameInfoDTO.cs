namespace CatalogOfGames.BAL.DTOs;

public class ShortGameInfoDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string MainImageDirectory { get; set; } = default!;
}