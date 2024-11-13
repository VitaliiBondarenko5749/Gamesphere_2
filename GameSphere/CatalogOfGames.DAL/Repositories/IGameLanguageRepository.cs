using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface IGameLanguageRepository : IGenericRepository<GameLanguage>
{
    Task<GameLanguage?> CheckExistenceAsync(Guid gameId, Guid languageId);
}

public class GameLanguageRepository : GenericRepository<GameLanguage>, IGameLanguageRepository
{
    public GameLanguageRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<GameLanguage?> CheckExistenceAsync(Guid gameId, Guid languageId)
    {
        return await dbContext.GameLanguages.SingleOrDefaultAsync(gl => gl.GameId.Equals(gameId) && gl.LanguageId.Equals(languageId));
    }
}