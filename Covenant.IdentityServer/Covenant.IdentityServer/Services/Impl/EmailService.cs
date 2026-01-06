using Covenant.IdentityServer.Services.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Covenant.IdentityServer.Services.Impl
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            _logger = logger;
            _emailSettings = options.Value;
        }

        public async Task<bool> SendEmail(string email, string subject, string message)
        {
            try
            {
                var mail = new MailMessage { From = new MailAddress(_emailSettings.FromEmail, Resources.Resources.CompanyName) };
                if (_emailSettings.Test)
                {
                    string[] emails = _emailSettings.TestEmails.Split(",") ?? new string[0];
                    if (!emails.Any()) return false;
                    foreach (string e in emails) mail.To.Add(e);
                }
                else
                {
                    string e = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;
                    mail.To.Add(new MailAddress(e));
                }
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;
                using (var smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending email to: {To}", email);
                return false;
            }
        }
    }
}