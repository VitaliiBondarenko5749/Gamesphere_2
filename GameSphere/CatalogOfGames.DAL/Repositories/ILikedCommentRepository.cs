using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogOfGames.DAL.Repositories;

public interface ILikedCommentRepository : IGenericRepository<LikedComment>
{
    Task<LikedComment?> CheckExistenceAsync(Guid commentId, string userId);
    Task<int> GetCountOfLikesAsync(Guid commentId);
    Task<LikedComment[]?> GetByUserIdAsync(string userId);
}

public class LikedCommentRepository : GenericRepository<LikedComment>, ILikedCommentRepository
{
    public LikedCommentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<LikedComment?> CheckExistenceAsync(Guid commentId, string userId)
    {
        return await dbContext.LikedComments.AsNoTracking()
            .SingleOrDefaultAsync(lc => lc.CommentId.Equals(commentId) && lc.UserId.Equals(userId));
    }

    public async Task<int> GetCountOfLikesAsync(Guid commentId)
    {
        return await dbContext.LikedComments.AsNoTracking()
            .CountAsync(lc => lc.CommentId.Equals(commentId));
    }

    public async Task<LikedComment[]?> GetByUserIdAsync(string userId)
    {
        return await dbContext.LikedComments.AsNoTracking()
            .Where(lc => lc.UserId.Equals(userId))
            .ToArrayAsync();
    }
}