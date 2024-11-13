using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

#pragma warning disable

public interface IGameRepository : IGenericRepository<Game>
{
    Task<IQueryable<Game>?> GetAllAsync();
    Task<IQueryable<Game>?> GetAllByCategoryNameAsync(string categoryName);
    Task<IQueryable<Game>?> GetFavoriteByUserIdAsync(string userId);
    Task<List<Game>> GetFavoriteGamesForRecAsync(string userId);
    Task<List<Game>> GetGamesByCategoriesAsync(List<string> categories);
    Task<Game?> GetFullGameDataByIdAsync(Guid gameId);
    Task<string[]?> GetImageDirectoryNamesByGameIdAsync(Guid gameId);
    Task<Game?> GetByNameAsync(string name);
    Task<Game[]?> GetTopTenByNameAsync(string name);
}

public class GameRepository : GenericRepository<Game>, IGameRepository
{
    public GameRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<IQueryable<Game>?> GetAllAsync() // Done.
    {
        return dbContext.Games.AsNoTracking()
            .AsQueryable();
    }

    public async Task<IQueryable<Game>?> GetAllByCategoryNameAsync(string categoryName) // Done.
    {
        return dbContext.Games.AsNoTracking()
            .Where(g => g.GameCategories.Any(gc => gc.Category.Name.Equals(categoryName)))
            .AsQueryable();
    }

    public async Task<IQueryable<Game>?> GetFavoriteByUserIdAsync(string userId) // Done
    {
        return dbContext.Games.AsNoTracking()
            .Where(g => g.FavoriteGames.Any(fg => fg.UserId.Equals(userId)))
            .AsQueryable();
    }

    public async Task<List<Game>> GetFavoriteGamesForRecAsync(string userId)
    {
        return await dbContext.FavoriteGames
        .Where(fg => fg.UserId == userId)
        .Include(fg => fg.Game)
            .ThenInclude(g => g.GameCategories)
            .ThenInclude(gc => gc.Category)
        .Select(fg => fg.Game)
        .ToListAsync();
    }

    public async Task<List<Game>> GetGamesByCategoriesAsync(List<string> categories)
    {
        return await dbContext.Games
            .Where(g => g.GameCategories.Any(gc => categories.Contains(gc.Category.Name)))
            .ToListAsync();
    }

    public async Task<Game?> GetFullGameDataByIdAsync(Guid gameId) // Done.
    {
        return await dbContext.Games.AsNoTracking()
            .Include(g => g.Publisher)
            .Include(g => g.GameCategories).ThenInclude(gc => gc.Category)
            .Include(g => g.GameDevelopers).ThenInclude(gd => gd.Developer)
            .Include(g => g.GameLanguages).ThenInclude(gl => gl.Language)
            .Include(g => g.GamePlatforms).ThenInclude(gp => gp.Platform)
            .Include(g => g.GameImages)
            .Include(g => g.GameVideoLinks)
            .SingleOrDefaultAsync(g => g.Id.Equals(gameId));
    }

    public async Task<string[]?> GetImageDirectoryNamesByGameIdAsync(Guid gameId) // Done.
    {
        return await dbContext.GameImages.AsNoTracking()
            .Where(gi => gi.GameId.Equals(gameId))
            .Select(gi => gi.ImageDirectory)
            .ToArrayAsync();
    }

    public async Task<Game?> GetByNameAsync(string name) // Done.
    {
        return await dbContext.Games.AsNoTracking()
            .SingleOrDefaultAsync(g => g.Name.Equals(name));
    }

    public async Task<Game[]?> GetTopTenByNameAsync(string name) // Done.
    {
        return await dbContext.Games.AsNoTracking()
            .Where(g => g.Name.Contains(name))
            .Take(10)
            .ToArrayAsync();
    }
}