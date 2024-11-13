using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface IFavoriteGameRepository : IGenericRepository<FavoriteGame>
{
    Task<FavoriteGame?> GetByUserIdAndGameIdAsync(string userId, Guid gameId);
    Task<FavoriteGame[]?> GetByUserIdAsync(string userId);
}

public class FavoriteGameRepository : GenericRepository<FavoriteGame>, IFavoriteGameRepository
{
    public FavoriteGameRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<FavoriteGame?> GetByUserIdAndGameIdAsync(string userId, Guid gameId)
    {
        return await dbContext.FavoriteGames.AsNoTracking()
            .SingleOrDefaultAsync(fg => fg.UserId.Equals(userId) && fg.GameId.Equals(gameId));
    }

    public async Task<FavoriteGame[]?> GetByUserIdAsync(string userId)
    {
        return await dbContext.FavoriteGames.AsNoTracking()
            .Where(fg => fg.UserId.Equals(userId))
            .ToArrayAsync();
    }
}