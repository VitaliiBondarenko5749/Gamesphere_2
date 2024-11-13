namespace Forum.BAL.DTOs;

public class SavePostDTO
{
    public Guid PostId { get; set; }
    public string UserId { get; set; } = null!;
}