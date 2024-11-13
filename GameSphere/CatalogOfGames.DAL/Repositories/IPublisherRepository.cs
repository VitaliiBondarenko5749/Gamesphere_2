using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface IPublisherRepository : IGenericRepository<Publisher>
{
    Task<Publisher?> GetByNameAsync(string name);
    Task<Publisher[]?> GetTopTenByNameAsync(string name);
}

public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
{
    public PublisherRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Publisher?> GetByNameAsync(string name)
    {
        return await dbContext.Publishers.AsNoTracking()
            .SingleOrDefaultAsync(p => p.Name.Equals(name));
    }

    public async Task<Publisher[]?> GetTopTenByNameAsync(string name)
    {
        return await dbContext.Publishers.AsNoTracking()
            .Where(p => p.Name.Contains(name))
            .Take(10)
            .ToArrayAsync();
    }
}