using Aggregator.Models.Authentication;

namespace Aggregator.Models.Forum;

public class FullReplyInfoDTO
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; } = null!;
    public string CreatedAt { get; set; } = null!;
    public Guid? ReplyToId { get; set; }
    public UserInfoDTO User { get; set; } = null!;
}