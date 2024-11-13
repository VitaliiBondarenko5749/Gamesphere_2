using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

#pragma warning disable

public interface IGameCommentRepository : IGenericRepository<GameComment>
{
    Task<IQueryable<GameComment>?> GetByGameIdAsync(Guid gameId);
    Task<GameComment[]?> GetByUserIdAsync(string userId);
}

public class GameCommentRepository : GenericRepository<GameComment>, IGameCommentRepository
{
    public GameCommentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<IQueryable<GameComment>?> GetByGameIdAsync(Guid gameId)
    {
        return dbContext.GameComments.AsNoTracking()
            .Where(gc => gc.GameId.Equals(gameId))
            .AsQueryable();
    }

    public async Task<GameComment[]?> GetByUserIdAsync(string userId)
    {
        return await dbContext.GameComments.AsNoTracking()
            .Where(gc => gc.UserId.Equals(userId))
            .ToArrayAsync();
    }
}