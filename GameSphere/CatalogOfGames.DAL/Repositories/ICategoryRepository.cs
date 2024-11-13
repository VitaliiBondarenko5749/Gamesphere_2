using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    new Task<Category[]?> GetAllAsync();
    Task<Category?> GetByNameAsync(string name);
    Task<Category[]?> GetTopTenByNameAsync(string name);
}

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async new Task<Category[]?> GetAllAsync()
    {
        return await dbContext.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .ToArrayAsync();
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await dbContext.Categories.AsNoTracking()
            .SingleOrDefaultAsync(c => c.Name.Equals(name));
    }

    public async Task<Category[]?> GetTopTenByNameAsync(string name)
    {
        return await dbContext.Categories.AsNoTracking()
            .Where(c => c.Name.Contains(name))
            .Take(10)
            .ToArrayAsync();
    }
}