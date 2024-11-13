using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface IGameCategoryRepository : IGenericRepository<GameCategory>
{
    Task<GameCategory?> CheckExistenceAsync(Guid gameId, Guid categoryId);
}

public class GameCategoryRepository : GenericRepository<GameCategory>, IGameCategoryRepository
{
    public GameCategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<GameCategory?> CheckExistenceAsync(Guid gameId, Guid categoryId)
    {
        return await dbContext.GameCategories.SingleOrDefaultAsync(gc => gc.GameId.Equals(gameId) && gc.CategoryId.Equals(categoryId));
    }
}