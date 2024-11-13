using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface IPlatformRepository : IGenericRepository<Platform>
{
    Task<Platform?> GetByNameAsync(string name);
    Task<Platform[]?> GetTopTenByNameAsync(string name);
}

public class PlatformRepository : GenericRepository<Platform>, IPlatformRepository
{
    public PlatformRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Platform?> GetByNameAsync(string name)
    {
        return await dbContext.Platforms.AsNoTracking()
            .SingleOrDefaultAsync(p => p.Name.Equals(name));
    }

    public async Task<Platform[]?> GetTopTenByNameAsync(string name)
    {
        return await dbContext.Platforms.AsNoTracking()
            .Where(p => p.Name.Contains(name))
            .Take(10)
            .ToArrayAsync();
    }
}