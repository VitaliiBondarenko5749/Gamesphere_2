using CatalogOfGames.DAL.Data.Configurations;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();    
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<GameVideoLink> GameVideoLinks => Set<GameVideoLink>();
    public DbSet<GameImage> GameImages => Set<GameImage>();
    public DbSet<FavoriteGame> FavoriteGames => Set<FavoriteGame>();    
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<GameCategory> GameCategories => Set<GameCategory>();
    public DbSet<Developer> Developers => Set<Developer>();
    public DbSet<GameDeveloper> GameDevelopers => Set<GameDeveloper>(); 
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<GamePlatform> GamePlatforms => Set<GamePlatform>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<GameLanguage> GameLanguages => Set<GameLanguage>();
    public DbSet<GameComment> GameComments => Set<GameComment>();   
    public DbSet<LikedComment> LikedComments => Set<LikedComment>();
    public DbSet<GameRating> GameRatings => Set<GameRating>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<GameStore> GameStores => Set<GameStore>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.;Initial Catalog=CatalogOfGamesDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gamecatalog");

        modelBuilder.ApplyConfiguration(new GameConfiguration());
        modelBuilder.ApplyConfiguration(new PublisherConfiguration());
        modelBuilder.ApplyConfiguration(new GameVideoLinkConfiguration());
        modelBuilder.ApplyConfiguration(new GameImageConfiguration());
        modelBuilder.ApplyConfiguration(new FavoriteGameConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new GameCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new DeveloperConfiguration());
        modelBuilder.ApplyConfiguration(new GameDeveloperConfiguration());
        modelBuilder.ApplyConfiguration(new PlatformConfiguration());
        modelBuilder.ApplyConfiguration(new GamePlatformConfiguration());
        modelBuilder.ApplyConfiguration(new LanguageConfiguration());
        modelBuilder.ApplyConfiguration(new GameLanguageConfiguration());
        modelBuilder.ApplyConfiguration(new GameCommentConfiguration());
        modelBuilder.ApplyConfiguration(new LikedCommentConfiguration());
        modelBuilder.ApplyConfiguration(new GameRatingConfiguration());
        modelBuilder.ApplyConfiguration(new StoreConfiguration());
        modelBuilder.ApplyConfiguration(new GameStoreConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}