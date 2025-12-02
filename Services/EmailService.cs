using MailKit.Net.Smtp;
using MimeKit;
using Project_X.Services;

namespace ASH_Translation.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            var from = Environment.GetEnvironmentVariable("EMAIL_FROM");
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using var smtp = new SmtpClient();
            try
            {
                var host = Environment.GetEnvironmentVariable("EMAIL_HOST");
                var portStr = Environment.GetEnvironmentVariable("EMAIL_PORT");
                int port = int.TryParse(portStr, out var parsedPort) ? parsedPort : 0;
                await smtp.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.SslOnConnect);

                var username = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
                var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
                await smtp.AuthenticateAsync(username, password);

                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}