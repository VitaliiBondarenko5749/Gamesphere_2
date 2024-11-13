using Forum.DAL.Entities;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Helpers.GeneralClasses.Forum.DTOs;

namespace Forum.DAL.Repositories;

public interface IReplyRepository : IGenericRepository<Reply>
{
    Task<int> GetCountOfPostReplies(Guid postId);
    Task<IEnumerable<Reply>?> GetAllAsync(GetRepliesPaginationDTO dto);
    Task<int> DeleteByReplyToReplyId(Guid replyId);
    Task DeleteRepliesRecursivelyAsync(Guid postId);
}

public class ReplyRepository : GenericRepository<Reply>, IReplyRepository
{
    public ReplyRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
        : base(sqlConnection, dbTransaction, "forum.Replies") { }

    public async Task<int> GetCountOfPostReplies(Guid postId)
    {
        return await sqlConnection.ExecuteScalarAsync<int>("SELECT COUNT(r.Id) FROM forum.Replies AS r WHERE r.PostId=@PostId",
            new { PostId = postId }, dbTransaction);
    }

    public async Task<IEnumerable<Reply>?> GetAllAsync(GetRepliesPaginationDTO dto)
    {
        string query = "SELECT * FROM forum.Replies AS r WHERE r.PostId=@PostId ORDER BY r.CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
        int offset = dto.Page > 0 ? (dto.Page - 1) * dto.PageSize : 0;
        var parameters = new { PostId = dto.PostId, Offset = offset, PageSize = dto.PageSize };

        return await sqlConnection.QueryAsync<Reply>(query, parameters, dbTransaction);
    }

    public async Task<int> DeleteByReplyToReplyId(Guid replyId)
    {
        string query = @"DELETE FROM forum.Replies WHERE ReplyToId=@ReplyToId;";
        var parameters = new
        {
            ReplyToId = replyId
        };

        return await sqlConnection.ExecuteAsync(query, parameters, dbTransaction);
    }

    public async Task DeleteRepliesRecursivelyAsync(Guid postId)
    {
        // Get all replies for the given post
        var replies = await sqlConnection.QueryAsync<Guid>(
            "SELECT Id FROM forum.Replies WHERE PostId = @PostId OR ReplyToId IN (SELECT Id FROM forum.Replies WHERE PostId = @PostId);",
            new { PostId = postId }, dbTransaction);

        foreach (var replyId in replies)
        {
            // Recursively delete child replies
            await DeleteRepliesRecursivelyAsync(replyId);
        }

        // Delete replies for the given post
        await sqlConnection.ExecuteAsync("DELETE FROM forum.Replies WHERE PostId = @PostId OR ReplyToId IN (SELECT Id FROM forum.Replies WHERE PostId = @PostId);",
            new { PostId = postId }, dbTransaction);
    }
}