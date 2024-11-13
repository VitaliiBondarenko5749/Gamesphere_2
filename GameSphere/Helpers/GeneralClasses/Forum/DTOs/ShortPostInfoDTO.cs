namespace Helpers.GeneralClasses.Forum.DTOs;

public class ShortPostInfoDTO
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int Views { get; set; }
    public int NumberOfReplies { get; set; }
}