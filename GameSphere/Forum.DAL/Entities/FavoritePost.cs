namespace Forum.DAL.Entities;

public class FavoritePost
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public Guid PostId { get; set; }
}