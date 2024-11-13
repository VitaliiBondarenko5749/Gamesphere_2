namespace Helpers.GeneralClasses.Forum.DTOs;

public class GetRepliesPaginationDTO
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public Guid PostId { get; set; }
}