using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Text;

namespace Forum.DAL.Repositories;

public interface IGenericRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task DeleteAsync(Guid id);
    Task<T> GetAsync(Guid id);
    Task UpdateAsync(T model);
    Task<Guid> AddAsync(T model);
}

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected SqlConnection sqlConnection;
    protected IDbTransaction dbTransaction;
    private readonly string tableName;

    protected GenericRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction, string tableName)
    {
        this.sqlConnection = sqlConnection;
        this.dbTransaction = dbTransaction;
        this.tableName = tableName;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await sqlConnection.QueryAsync<T>($"SELECT * FROM {tableName};", null, dbTransaction);
    }

    public async Task<T> GetAsync(Guid id)
    {
        return await sqlConnection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {tableName} WHERE Id=@Id;",
            param: new { Id = id }, transaction: dbTransaction)
            ?? throw new KeyNotFoundException($"{tableName} with id [{id}] could not be found.");
    }

    public async Task DeleteAsync(Guid id)
    {
        await sqlConnection.ExecuteAsync($"DELETE FROM {tableName} WHERE Id=@Id;", param: new { Id = id },
            transaction: dbTransaction);
    }

    public async Task<Guid> AddAsync(T model)
    {
        string insertQuery = GenerateInsertQuery();

        return await sqlConnection.ExecuteScalarAsync<Guid>(insertQuery, param: model, transaction: dbTransaction);
    }

    public async Task UpdateAsync(T model)
    {
        string updateQuery = GenerateUpdateQuery();

        await sqlConnection.ExecuteAsync(updateQuery, param: model, transaction: dbTransaction);
    }

    // Генерація INSERT запиту
    private string GenerateInsertQuery()
    {
        StringBuilder insertQuery = new StringBuilder($"INSERT INTO {tableName} (");
        List<string> properties = GenerateListOfProperties(GetProperties);

        properties.ForEach(property => { insertQuery.Append($"[{property}],"); });

        insertQuery.Remove(insertQuery.Length - 1, 1).Append(") VALUES (");

        properties.ForEach(property => { insertQuery.Append($"@{property},"); });

        insertQuery.Remove(insertQuery.Length - 1, 1).Append("); SELECT SCOPE_IDENTITY();");

        return insertQuery.ToString();
    }

    // Отримання списку властивостей
    private IEnumerable<PropertyInfo> GetProperties
    {
        get
        {
            return typeof(T).GetProperties()
               .Where(prop => prop.GetCustomAttributes(false)
               .All(attr => attr.GetType().Namespace != "AutoMapper.Configuration.Annotations"
               || attr.GetType().Name != "IgnoreAttribute"));
        }
    }

    // Генерація списку властивостей
    private List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> propertyList)
    {
        return propertyList.Select(p => p.Name).ToList();
    }

    // Генерація UPDATE запиту
    private string GenerateUpdateQuery()
    {
        StringBuilder updateQuery = new StringBuilder($"UPDATE {tableName} SET ");
        List<string> properties = GenerateListOfProperties(GetProperties);

        properties.ForEach(property =>
        {
            updateQuery.Append($"{property}=@{property},");
        });

        updateQuery.Remove(updateQuery.Length - 1, 1).Append(" WHERE Id = @Id;");

        return updateQuery.ToString();
    }
}