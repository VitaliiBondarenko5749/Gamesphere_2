using System.Reflection.Emit;

namespace CatalogOfGames.BAL.DTOs;

public class CommentInfoDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string CreatedAt { get; set; } = default!;
    public string UserId { get; set; } = default!;
}