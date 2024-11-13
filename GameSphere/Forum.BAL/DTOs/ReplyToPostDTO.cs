namespace Forum.BAL.DTOs;

public class ReplyToPostDTO
{
    public string UserId { get; set; } = null!;
    public Guid PostId { get; set; }
    public string Content { get; set; } = null!;
    public Guid? ReplyToId { get; set; }
}