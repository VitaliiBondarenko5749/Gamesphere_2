using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using EmailService.API.ViewModels;

namespace EmailService.API.Services;

public interface IEmailSenderService
{
    Task SendEmailAsync(SendEmailViewModel model);
}

public class EmailSenderService : IEmailSenderService
{
    public async Task SendEmailAsync(SendEmailViewModel model)
    {
        using MimeMessage emailMessage = new();

        emailMessage.From.Add(new MailboxAddress("GameSphere", "gamesphere7438@gmail.com"));
        emailMessage.To.Add(new MailboxAddress(string.Empty, model.SendTo));
        emailMessage.Subject = model.Subject; // It's a title.
        emailMessage.Body = new TextPart((model.IsHtml) ? TextFormat.Html : TextFormat.Text)
        {
            Text = model.Message
        };

        using (SmtpClient client = new())
        {
            await client.ConnectAsync("smtp.gmail.com", 465, true);
            await client.AuthenticateAsync("gamesphere7438@gmail.com", "qevt tswh rqrw niun");
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }
}