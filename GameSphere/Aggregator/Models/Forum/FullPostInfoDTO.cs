using Aggregator.Models.Authentication;
using Helpers.GeneralClasses.Forum.DTOs;

namespace Aggregator.Models.Forum;

public class FullPostInfoDTO
{
    public Guid Id { get; set; }
    public string Topic { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string CreatedAt { get; set; } = null!;
    public int Views { get; set; } = 0;
    public UserInfoDTO User { get; set; } = null!;
    public PostGameInfoDTO? Game { get; set; } = null!;
}