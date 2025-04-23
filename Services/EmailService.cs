using System.Net.Mail;
using System.Net;

namespace ExpenseTracker.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SMTPServer"];
            var port = int.Parse(_configuration["EmailSettings:SMTPPort"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];

            using (var client = new SmtpClient(smtpServer, port))
            {
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                try
                {
                    _logger.LogInformation("Sending email to {Email}...", toEmail);
                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error sending email: {ErrorMessage}", ex.Message);
                }
            }
        }
    }
}
