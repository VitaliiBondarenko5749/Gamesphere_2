using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface IGameDeveloperRepository : IGenericRepository<GameDeveloper>
{
    Task<GameDeveloper?> CheckExistenceAsync(Guid gameId, Guid developerId);
}

public class GameDeveloperRepository : GenericRepository<GameDeveloper>, IGameDeveloperRepository
{
    public GameDeveloperRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<GameDeveloper?> CheckExistenceAsync(Guid gameId, Guid developerId)
    {
        return await dbContext.GameDevelopers.SingleOrDefaultAsync(gd => gd.GameId.Equals(gameId) && gd.DeveloperId.Equals(developerId));
    }
}