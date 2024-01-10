using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
namespace TaskManagmentAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailKitOptions _mailKitOptions;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailKitOptions> mailKitOptions, ILogger<EmailService> logger)
        {
            _mailKitOptions = mailKitOptions.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_mailKitOptions.SenderName, _mailKitOptions.SenderEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_mailKitOptions.Server, _mailKitOptions.Port, false);
                await client.AuthenticateAsync(_mailKitOptions.Username, _mailKitOptions.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {to}. Error: {ex.Message}");
                return false;
            }
        }
    }
}
