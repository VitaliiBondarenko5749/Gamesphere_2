namespace Helpers;

public class ServerResponse
{
    public string Message { get; set; } = default!;
    public bool IsSuccess { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public DateTime? ExpiredDate { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}