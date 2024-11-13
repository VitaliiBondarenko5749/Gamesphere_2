using Forum.DAL.Entities;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace Forum.DAL.Repositories;

public interface IFavoritePostRepository : IGenericRepository<FavoritePost> 
{
    Task<int> DeleteByPostAndUserIdAsync(Guid postId, string userId);
    Task<FavoritePost?> GetByPostAndUserIdAsync(Guid postId, string userId);
}

public class FavoritePostRepository : GenericRepository<FavoritePost>, IFavoritePostRepository
{
    public FavoritePostRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
        : base(sqlConnection, dbTransaction, "forum.FavoritePosts") { }

    public async Task<int> DeleteByPostAndUserIdAsync(Guid postId, string userId)
    {
        string query = @"DELETE FROM forum.FavoritePosts WHERE PostId=@PostId AND UserId=@UserId;";
        var parameters = new
        {
            PostId = postId,
            UserId = userId
        };

        return await sqlConnection.ExecuteAsync(query, parameters, dbTransaction);
    }

    public async Task<FavoritePost?> GetByPostAndUserIdAsync(Guid postId, string userId)
    {
        string query = @"SELECT TOP 1 * FROM forum.FavoritePosts AS fp WHERE fp.PostId=@PostId AND fp.UserId=@UserId;";
        var parameters = new
        {
            PostId = postId,
            UserId = userId
        };

        return await sqlConnection.QuerySingleOrDefaultAsync<FavoritePost?>(query, parameters, dbTransaction);
    }
}