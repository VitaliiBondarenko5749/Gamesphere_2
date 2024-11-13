using Helpers.GeneralClasses.Forum.Enums;

namespace Helpers.GeneralClasses.Forum.DTOs;

public class GetTopicsPaginationDTO
{
    public int? Page { get; set; }
    public int PageSize { get; set; }
    public TopicViewer TopicViewer { get; set; } = TopicViewer.All;
    public TopicSorter TopicSorter { get; set; } = TopicSorter.DateByDescending;
    public string SearchText { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? GameIds { get; set; }
}