namespace Aggregator.Services;

public interface IEmailService
{
    Task SendAsync(string sendTo, string subject, string message, bool isHtml);
}

public class EmailService : IEmailService
{
    private readonly HttpClient httpClient;

    public EmailService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task SendAsync(string sendTo, string subject, string message, bool isHtml)
    {
        var requestData = new
        {
            SendTo = sendTo,
            Subject = subject,
            Message = message,
            IsHtml = isHtml
        };

        await httpClient.PostAsJsonAsync("/api/Email", requestData);
    }
}