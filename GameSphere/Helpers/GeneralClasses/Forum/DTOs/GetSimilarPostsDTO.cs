namespace Helpers.GeneralClasses.Forum.DTOs;

public class GetSimilarPostsDTO
{
    public Guid PostId { get; set; }
    public string CurrentPostText { get; set; } = null!;
    public Guid? GameId { get; set; }
    public int Count { get; set; }
}