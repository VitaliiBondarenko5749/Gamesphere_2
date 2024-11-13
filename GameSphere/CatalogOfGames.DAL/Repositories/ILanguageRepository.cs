using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface ILanguageRepository : IGenericRepository<Language>
{
    Task<Language?> GetByNameAsync(string name);
    Task<Language[]?> GetTopTenByNameAsync(string name);    
}

public class LanguageRepository : GenericRepository<Language>, ILanguageRepository
{
    public LanguageRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Language?> GetByNameAsync(string name)
    {
        return await dbContext.Languages.AsNoTracking()
            .SingleOrDefaultAsync(l => l.Name.Equals(name));
    }

    public async Task<Language[]?> GetTopTenByNameAsync(string name)
    {
        return await dbContext.Languages.AsNoTracking()
            .Where(l => l.Name.Contains(name))
            .Take(10)
            .ToArrayAsync();
    }
}