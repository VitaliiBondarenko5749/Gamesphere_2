using Forum.DAL.Entities;
using System.Data.SqlClient;
using System.Data;
using Helpers.GeneralClasses.Forum.Enums;
using Helpers.GeneralClasses.Forum.DTOs;
using System.Text;
using Dapper;

namespace Forum.DAL.Repositories;

public interface IPostRepository : IGenericRepository<Post> 
{
    Task<IEnumerable<Post>?> GetAllAsync(GetTopicsPaginationDTO dto);
    Task<int> GetCountOfPostsAsync(GetTopicsPaginationDTO dto);
    Task<int> UpdateViewsAsync(Guid postId, int value);
    Task<IEnumerable<Guid>?> GetPostIdsByUserAsync(string userId);
}

public class PostRepository : GenericRepository<Post>, IPostRepository
{
    public PostRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
        : base(sqlConnection, dbTransaction, "forum.Posts") { }

    public async Task<IEnumerable<Post>?> GetAllAsync(GetTopicsPaginationDTO dto)
    {
        StringBuilder queryStringBuilder = new();

        // TopicViewer
        switch (dto.TopicViewer)
        {
            case TopicViewer.All:
                {
                    queryStringBuilder.Append("SELECT * FROM forum.Posts AS p");
                    break;
                }
            case TopicViewer.Saved:
                {
                    queryStringBuilder.Append("SELECT * FROM forum.Posts AS p JOIN forum.FavoritePosts AS fp ON p.Id = fp.PostId WHERE fp.UserId=@UserId");
                    break;
                }
            case TopicViewer.Own:
                {
                    queryStringBuilder.Append("SELECT * FROM forum.Posts AS p WHERE p.UserId = @UserId");
                    break;
                }
            default: throw new ArgumentException("Querry URL error!");
        }

        // SearchText
        if (!string.IsNullOrEmpty(dto.SearchText) && queryStringBuilder.ToString().Contains("WHERE"))
        {
            queryStringBuilder.Append($" AND LOWER(p.Topic) LIKE '%' + LOWER(@SearchText) + '%'");
        }
        else if (!string.IsNullOrEmpty(dto.SearchText))
        {
            queryStringBuilder.Append($" WHERE LOWER(p.Topic) LIKE '%' + LOWER(@SearchText) + '%'");
        }

        string[]? gameIds = dto.GameIds?.Split(',');

        // GameIds
        if (gameIds is not null && queryStringBuilder.ToString().Contains("WHERE"))
        {
            queryStringBuilder.Append(" AND p.GameId IN @GameIds");
        }
        else if(gameIds is not null && !queryStringBuilder.ToString().Contains("WHERE"))
        {
            queryStringBuilder.Append(" WHERE p.GameId IN @GameIds");
        }

        //TopicSorter
        switch (dto.TopicSorter)
        {
            case TopicSorter.DateByAscending:
                {
                    queryStringBuilder.Append(" ORDER BY p.CreatedAt ASC, p.Id ASC");
                    break;
                }
            case TopicSorter.ViewsByDescending:
                {
                    queryStringBuilder.Append(" ORDER BY p.Views DESC, p.Id ASC");
                    break;
                }
            case TopicSorter.ViewsByAscending:
                {
                    queryStringBuilder.Append(" ORDER BY p.Views ASC, p.Id ASC");
                    break;
                }
            default:
                {
                    queryStringBuilder.Append(" ORDER BY p.CreatedAt DESC, p.Id ASC");
                    break;
                }
        }

        int offset = (dto.Page.HasValue && dto.Page.Value > 0) ? (dto.Page.Value - 1) * dto.PageSize : 0;

        queryStringBuilder.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;");
      
        string query = queryStringBuilder.ToString();

        var parameters = new
        {
            UserId = dto.UserId,
            SearchText = dto.SearchText.ToLower(),
            GameIds = gameIds,
            Offset = offset,
            PageSize = dto.PageSize
        };

        return await sqlConnection.QueryAsync<Post>(query, parameters, dbTransaction);
    }

    public async Task<int> GetCountOfPostsAsync(GetTopicsPaginationDTO dto)
    {
        StringBuilder queryStringBuilder = new();

        // TopicViewer
        switch (dto.TopicViewer)
        {
            case TopicViewer.All:
                {
                    queryStringBuilder.Append("SELECT COUNT(p.Id) FROM forum.Posts AS p");
                    break;
                }
            case TopicViewer.Saved:
                {
                    queryStringBuilder.Append("SELECT COUNT(p.Id) FROM forum.Posts AS p JOIN forum.FavoritePosts AS fp ON p.Id = fp.PostId WHERE fp.UserId=@UserId");
                    break;
                }
            case TopicViewer.Own:
                {
                    queryStringBuilder.Append("SELECT COUNT(p.Id) FROM forum.Posts AS p WHERE p.UserId = @UserId");
                    break;
                }
            default: throw new ArgumentException("Querry URL error!");
        }

        // SearchText
        if (!string.IsNullOrEmpty(dto.SearchText) && queryStringBuilder.ToString().Contains("WHERE"))
        {
            queryStringBuilder.Append($" AND LOWER(p.Topic) LIKE '%' + LOWER(@SearchText) + '%'");
        }
        else if (!string.IsNullOrEmpty(dto.SearchText))
        {
            queryStringBuilder.Append($" WHERE LOWER(p.Topic) LIKE '%' + LOWER(@SearchText) + '%'");
        }

        string[]? gameIds = dto.GameIds?.Split(',');

        // GameIds
        if (gameIds is not null && queryStringBuilder.ToString().Contains("WHERE"))
        {
            queryStringBuilder.Append(" AND p.GameId IN @GameIds");
        }
        else if (gameIds is not null && !queryStringBuilder.ToString().Contains("WHERE"))
        {
            queryStringBuilder.Append(" WHERE p.GameId IN @GameIds");
        }

        queryStringBuilder.Append(';');

        string query = queryStringBuilder.ToString();

        var parameters = new
        {
            UserId = dto.UserId,
            SearchText = dto.SearchText.ToLower(),
            GameIds = gameIds,
        };

        return await sqlConnection.ExecuteScalarAsync<int>(query, parameters, dbTransaction);
    }

    public async Task<int> UpdateViewsAsync(Guid postId, int value)
    {
        string query = @"UPDATE forum.Posts SET Views=@Views WHERE Id=@PostId;";

        var parameters = new
        {
            Views = value,
            PostId = postId
        };

        // Повертаємо кількість заповнених рядків
        return await sqlConnection.ExecuteAsync(query, parameters, dbTransaction);
    }

    public async Task<IEnumerable<Guid>?> GetPostIdsByUserAsync(string userId)
    {
        string query = @"SELECT p.Id from forum.Posts AS p WHERE UserId=@UserId;";
        var parameters = new { UserId = userId };

        return await sqlConnection.QueryAsync<Guid>(query, parameters, dbTransaction);
    }
}