using CRS.Infrastructure.Interface;
using System.Net.Mail;
using System.Net;

namespace CRS.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");

            using (var smtpClient = new SmtpClient
            {
                Host = smtpSettings["SmtpServer"],
                Port = int.Parse(smtpSettings["Port"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    smtpSettings["SenderEmail"],
                    smtpSettings["Password"]
                ),
                Timeout = 10000 // 10 seconds
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(
                        smtpSettings["SenderEmail"],
                        smtpSettings["SenderName"]
                    ),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (SmtpException ex)
                {
                    throw new InvalidOperationException($"SMTP Error: {ex.StatusCode}. {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Email failed to send: {ex.Message}", ex);
                }
            }
        }
    }
}
