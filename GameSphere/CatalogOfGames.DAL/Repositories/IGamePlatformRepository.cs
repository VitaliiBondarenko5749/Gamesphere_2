using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface IGamePlatformRepository : IGenericRepository<GamePlatform>
{
    Task<GamePlatform?> CheckExistenceAsync(Guid gameId, Guid platformId);
}

public class GamePlatformRepository : GenericRepository<GamePlatform>, IGamePlatformRepository
{
    public GamePlatformRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<GamePlatform?> CheckExistenceAsync(Guid gameId, Guid platformId)
    {
        return await dbContext.GamePlatforms.SingleOrDefaultAsync(gp => gp.GameId.Equals(gameId) && gp.PlatformId.Equals(platformId));
    }
}