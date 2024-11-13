namespace CatalogOfGames.BAL.DTOs;

public class FullGameInfoDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string ReleaseDate { get; set; } = default!;
    public string Description { get; set; } = null!;
    public string? MainImageDirectory { get; set; }
    public int Views { get; set; }
    public string PublisherName { get; set; } = null!;
    public ICollection<string>? Categories { get; set; }
    public ICollection<string>? Developers { get; set; }
    public ICollection<string>? Languages { get; set; }
    public ICollection<string>? Platforms { get; set; }
    public ICollection<string>? Images { get; set; }
    public ICollection<string>? VideoLinks { get; set; }
}