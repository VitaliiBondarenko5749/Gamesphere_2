namespace Forum.BAL.DTOs;

public class AddPostDTO
{
    public string Subject { get; set; } = null!;
    public string Text { get; set; } = null!;
    public Guid? GameId { get; set; }
    public string UserId { get; set; } = null!;
}