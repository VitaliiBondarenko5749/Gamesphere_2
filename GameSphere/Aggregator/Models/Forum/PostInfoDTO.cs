namespace Aggregator.Models.Forum;

public class PostInfoDTO
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public Guid? GameId { get; set; }
    public string Topic { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int Views { get; set; } = 0;
}