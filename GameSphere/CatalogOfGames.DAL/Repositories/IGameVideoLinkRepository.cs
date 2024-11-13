using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;

namespace CatalogOfGames.DAL.Repositories;

public interface IGameVideoLinkRepository : IGenericRepository<GameVideoLink>
{
}

public class GameVideoLinkRepository : GenericRepository<GameVideoLink>, IGameVideoLinkRepository
{
    public GameVideoLinkRepository(ApplicationDbContext dbContext) : base(dbContext) { }
}