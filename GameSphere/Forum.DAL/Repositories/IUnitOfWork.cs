using System.Data;

namespace Forum.DAL.Repositories;

public interface IUnitOfWork : IDisposable 
{
    IPostRepository PostRepository { get; }
    IFavoritePostRepository FavoritePostRepository { get; }
    IReplyRepository ReplyRepository { get; }

    void Commit();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbTransaction dbTransaction;

    public UnitOfWork(IPostRepository postRepository, IFavoritePostRepository favoritePostRepository, 
        IReplyRepository replyRepository, IDbTransaction dbTransaction)
    {
        PostRepository = postRepository;
        FavoritePostRepository = favoritePostRepository;
        ReplyRepository = replyRepository;

        this.dbTransaction = dbTransaction;
    }

    public IPostRepository PostRepository { get; }
    public IFavoritePostRepository FavoritePostRepository { get; }
    public IReplyRepository ReplyRepository { get; }

    public void Commit()
    {
        try
        {
            dbTransaction.Commit();
        }
        catch (Exception ex)
        {
            dbTransaction.Rollback();

            Console.WriteLine(ex.Message);
        }
    }

    public void Dispose() 
    {
        dbTransaction.Connection?.Close();
        dbTransaction.Connection?.Dispose();
        dbTransaction.Dispose();
    }
}