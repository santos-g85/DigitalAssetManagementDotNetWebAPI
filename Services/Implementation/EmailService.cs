using DAMApi.Models.DTOs;
using DAMApi.Services.Interfaces;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace DAMApi.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

    
        public void SendEmail(EmailReceiverDto request)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                email.Subject = request.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = request.Body };
                foreach (var recipient in request.To)
                {
                    email.To.Add(MailboxAddress.Parse(recipient));
                }
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                _logger.LogInformation("Connecting to SMTP server: {Host}", _config.GetSection("EmailHost").Value);
                smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                smtp.Send(email);
                _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", request.To));
                smtp.Disconnect(true);
                _logger.LogInformation("Disconnected from SMTP server: {Host}", _config.GetSection("EmailHost").Value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while sending the email: {ex.Message}", ex);
                throw new InvalidOperationException("Failed to send email", ex);
            }

        }
    }
}


