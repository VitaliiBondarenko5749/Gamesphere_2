namespace Helpers.GeneralClasses.Forum.DTOs;

public class PostGameInfoDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string MainImageDirectory { get; set; } = null!;
}