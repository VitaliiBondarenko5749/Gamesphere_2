using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface IGameImageRepository : IGenericRepository<GameImage>
{
    Task<List<string>?> GetDirectoriesByGameIdAsync(Guid gameId);
}

public class GameImageRepository : GenericRepository<GameImage>, IGameImageRepository
{
    public GameImageRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<List<string>?> GetDirectoriesByGameIdAsync(Guid gameId)
    {
        return await dbContext.GameImages.AsNoTracking()
            .Where(gi => gi.GameId.Equals(gameId))
            .Select(gi => gi.ImageDirectory)
            .ToListAsync();
    }
}