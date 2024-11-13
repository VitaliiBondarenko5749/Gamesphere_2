using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface IDeveloperRepository : IGenericRepository<Developer>
{
    Task<Developer?> GetByNameAsync(string name);
    Task<Developer[]?> GetTopTenByNameAsync(string name);
}

public class DeveloperRepository : GenericRepository<Developer>, IDeveloperRepository
{
    public DeveloperRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Developer?> GetByNameAsync(string name)
    {
        return await dbContext.Developers.AsNoTracking()
            .SingleOrDefaultAsync(d => d.Name.Equals(name));
    }

    public async Task<Developer[]?> GetTopTenByNameAsync(string name)
    {
        return await dbContext.Developers.AsNoTracking()
            .Where(d => d.Name.Contains(name))
            .Take(10)
            .ToArrayAsync();
    }
}