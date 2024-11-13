using CatalogOfGames.DAL.Data;

namespace CatalogOfGames.DAL.Repositories;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get; }
    IGameRepository GameRepository { get; }
    IFavoriteGameRepository FavoriteGameRepository { get; }
    IDeveloperRepository DeveloperRepository { get; }
    ILanguageRepository LanguageRepository { get; }
    IPlatformRepository PlatformRepository { get; }
    IPublisherRepository PublisherRepository { get; }
    IGameCommentRepository GameCommentRepository { get; }
    ILikedCommentRepository LikedCommentRepository { get; }
    IGameImageRepository GameImageRepository { get; }
    IGameVideoLinkRepository GameVideoLinkRepository { get; }
    IGameCategoryRepository GameCategoryRepository { get; }
    IGameDeveloperRepository GameDeveloperRepository { get; }
    IGameLanguageRepository GameLanguageRepository { get; }
    IGamePlatformRepository GamePlatformRepository { get; }

    Task SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    protected readonly ApplicationDbContext dbContext;

    public UnitOfWork(ApplicationDbContext dbContext, ICategoryRepository categoryRepository,
        IGameRepository gameRepository, IFavoriteGameRepository favoriteGameRepository, IDeveloperRepository developerRepository, 
        ILanguageRepository languageRepository, IPlatformRepository platformRepository, IPublisherRepository publisherRepository,
        IGameCommentRepository gameCommentRepository, ILikedCommentRepository likedCommentRepository, IGameImageRepository gameImageRepository, 
        IGameVideoLinkRepository gameVideoLinkRepository, IGameCategoryRepository gameCategoryRepository, IGameDeveloperRepository gameDeveloperRepository,
        IGameLanguageRepository gameLanguageRepository, IGamePlatformRepository gamePlatformRepository)
    {
        this.dbContext = dbContext;
        CategoryRepository = categoryRepository;
        GameRepository = gameRepository;
        FavoriteGameRepository = favoriteGameRepository;
        DeveloperRepository = developerRepository;
        LanguageRepository = languageRepository;
        PlatformRepository = platformRepository;
        PublisherRepository = publisherRepository;
        GameCommentRepository = gameCommentRepository;
        LikedCommentRepository = likedCommentRepository;
        GameImageRepository = gameImageRepository;
        GameVideoLinkRepository = gameVideoLinkRepository;
        GameCategoryRepository = gameCategoryRepository;
        GameDeveloperRepository = gameDeveloperRepository;
        GameLanguageRepository = gameLanguageRepository;
        GamePlatformRepository = gamePlatformRepository;
    }

    public ICategoryRepository CategoryRepository { get; }
    public IGameRepository GameRepository { get; }
    public IFavoriteGameRepository FavoriteGameRepository { get; }
    public IDeveloperRepository DeveloperRepository { get; }
    public ILanguageRepository LanguageRepository { get; }
    public IPlatformRepository PlatformRepository { get; }
    public IPublisherRepository PublisherRepository { get; }
    public IGameCommentRepository GameCommentRepository { get; }
    public ILikedCommentRepository LikedCommentRepository { get; }
    public IGameImageRepository GameImageRepository { get; }
    public IGameVideoLinkRepository GameVideoLinkRepository { get; }
    public IGameCategoryRepository GameCategoryRepository { get; }
    public IGameDeveloperRepository GameDeveloperRepository { get; }
    public IGameLanguageRepository GameLanguageRepository { get; }
    public IGamePlatformRepository GamePlatformRepository { get; }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}