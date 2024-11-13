namespace CatalogOfGames.DAL.Entities;

public class Game
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public string Description { get; set; } = null!;
    public string? MainImageDirectory { get; set; }
    public int Views { get; set; }

    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;

    public ICollection<GameCategory> GameCategories { get; set; } = null!;  

    public ICollection<GameDeveloper> GameDevelopers { get; set; } = null!;

    public ICollection<GameLanguage> GameLanguages { get; set; } = null!; 

    public ICollection<GamePlatform> GamePlatforms { get; set; } = null!;

    public ICollection<FavoriteGame> FavoriteGames { get; set; } = null!;

    public ICollection<GameComment> GameComments { get; set; } = null!;

    public ICollection<GameImage> GameImages { get; set; } = null!; 

    public ICollection<GameVideoLink> GameVideoLinks { get; set; } = null!;

    public ICollection<GameRating> GameRatings { get; set; } = null!;

    public ICollection<GameStore> GameStores { get; set; } = null!;
}