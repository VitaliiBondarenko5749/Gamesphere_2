using CatalogOfGames.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task<T> CreateAsync(T entity);
    void UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    void DeleteRange(T[] entities);
}

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext dbContext;
    private readonly DbSet<T> table;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        table = dbContext.Set<T>();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await table.AddAsync(entity);

        return entity;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await table.ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await table.FindAsync(id)
            ?? throw new NullReferenceException("Entity type is nullable!");
    }

    public void UpdateAsync(T entity)
    {
        dbContext.Update(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        T? entity = await table.FindAsync(id)
            ?? throw new NullReferenceException($"Could not find Entity by id: {id}");

        table.Remove(entity);
    }

    public void DeleteRange(T[] entities)
    {
        table.RemoveRange(entities);
    }
}