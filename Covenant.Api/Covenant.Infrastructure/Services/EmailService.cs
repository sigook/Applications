using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Covenant.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IEnumerable<EmailSettings> _emailSettings;

    public EmailService(IOptions<List<EmailSettings>> options, ILogger<EmailService> logger)
    {
        _logger = logger;
        _emailSettings = options.Value;
    }

    public async Task<bool> SendEmail(EmailParams emailParams)
    {
        try
        {
            var settings = _emailSettings.FirstOrDefault(e => e.EmailSettingName == emailParams.EmailSettingName);
            if (settings is null) return false;
            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(settings.DisplayName, settings.FromEmail));
            if (settings.Test)
            {
                string[] emails = settings.TestEmails.Split(",") ?? Array.Empty<string>();
                if (!emails.Any())
                {
                    return false;
                }
                foreach (string e in emails)
                {
                    mail.To.Add(new MailboxAddress(string.Empty, e));
                }
            }
            else
            {
                string e = string.IsNullOrEmpty(emailParams.Email) ? settings.ToEmail : emailParams.Email;
                mail.To.Add(new MailboxAddress(string.Empty, e));
                foreach (string cc in emailParams.Cc)
                {
                    mail.Cc.Add(new MailboxAddress(string.Empty, cc));
                }
                foreach (string bcc in emailParams.Bcc)
                {
                    mail.Bcc.Add(new MailboxAddress(string.Empty, bcc));
                }
                var ccEmails = string.IsNullOrEmpty(settings.CcEmail) ? Array.Empty<string>() : settings.CcEmail.Split(",");
                foreach (string ccEmail in ccEmails)
                {
                    mail.Bcc.Add(new MailboxAddress(string.Empty, ccEmail));
                }
            }
            mail.Subject = emailParams.Subject;
            var builder = new BodyBuilder
            {
                HtmlBody = emailParams.Message
            };
            foreach (var attachment in emailParams.Attachments)
            {
                if (attachment.IsPath)
                    builder.Attachments.Add(attachment.FilePath, ContentType.Parse(attachment.MediaType));
                else
                    builder.Attachments.Add(attachment.Name, attachment.ContentStream, ContentType.Parse(attachment.MediaType));
            }
            mail.Body = builder.ToMessageBody();
            mail.Priority = MessagePriority.Normal;
            using var client = new SmtpClient();
            await client.ConnectAsync(settings.PrimaryDomain, settings.PrimaryPort, false);
            await client.AuthenticateAsync(settings.Username, settings.Password);
            var response = await client.SendAsync(mail);
            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("ERROR SENDING EMAIL********************************************");
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}