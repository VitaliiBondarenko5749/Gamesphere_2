namespace Aggregator.Models.Forum;

public class ShortPostInfoDTO
{
    public Guid Id { get; set; }
    public string Topic { get; set; } = null!;
    public string CreatedAt { get; set; } = null!;
    public int Views { get; set; }
    public int NumberOfReplies { get; set; }

    public ForumUserInfoDTO UserInfo { get; set; } = null!;
}