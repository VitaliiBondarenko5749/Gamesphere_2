namespace JWTAuthentication.ViewModels;

public class GetUsersPaginationViewModel
{
    public string SearchText { get; set; } = string.Empty;
    public int? Page { get; set; }
    public int PageSize { get; set; }
}