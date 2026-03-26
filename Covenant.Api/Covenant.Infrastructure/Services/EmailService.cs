using Azure.Identity;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using MimeKit;
using ContentType = MimeKit.ContentType;
using EmailSettings = Covenant.Common.Configuration.EmailSettings;

namespace Covenant.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IEnumerable<EmailSettings> _emailSettings;
    private readonly Microsoft365Configuration _microsoft365Config;
    private readonly GraphServiceClient _graphClient;

    public EmailService(
        IOptions<List<EmailSettings>> options,
        ILogger<EmailService> logger,
        IOptions<Microsoft365Configuration> microsoft365Options)
    {
        _logger = logger;
        _emailSettings = options.Value;

        _microsoft365Config = microsoft365Options.Value;
        var credentials = new ClientSecretCredential(
            _microsoft365Config.TenantId,
            _microsoft365Config.ClientId,
            _microsoft365Config.ClientSecret);
        _graphClient = new GraphServiceClient(credentials);
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
                string[] emails = settings.TestEmails.Split(",") ?? [];
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
                var ccEmails = string.IsNullOrEmpty(settings.CcEmail) ? [] : settings.CcEmail.Split(",");
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
            var password = string.IsNullOrWhiteSpace(settings.Provider) ? settings.Password : $"{settings.Provider}{settings.Password}";
            using var client = new SmtpClient();
            await client.ConnectAsync(settings.PrimaryDomain, settings.PrimaryPort, false);
            await client.AuthenticateAsync(settings.Username, password);
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

    public async Task<bool> SendCovenantEmail(EmailParams emailParams)
    {
        try
        {
            var settings = _emailSettings.FirstOrDefault(e => e.EmailSettingName == emailParams.EmailSettingName);
            if (settings is null)
            {
                _logger.LogError("EmailSettings not found for {EmailSettingName}", emailParams.EmailSettingName);
                return false;
            }
            if (!string.IsNullOrWhiteSpace(settings.Password))
            {
                return await SendEmail(emailParams);
            }
            var message = new Message
            {
                Subject = emailParams.Subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = emailParams.Message
                },
                From = new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = settings.FromEmail,
                        Name = settings.DisplayName
                    }
                },
                ToRecipients = new List<Recipient>(),
                CcRecipients = new List<Recipient>(),
                BccRecipients = new List<Recipient>(),
                Attachments = new List<Attachment>()
            };
            if (settings.Test)
            {
                string[] testEmails = settings.TestEmails?.Split(",") ?? [];
                foreach (var email in testEmails)
                {
                    message.ToRecipients.Add(new Recipient
                    {
                        EmailAddress = new EmailAddress { Address = email.Trim() }
                    });
                }
            }
            else
            {
                string recipientEmail = string.IsNullOrEmpty(emailParams.Email) ? settings.ToEmail : emailParams.Email;
                message.ToRecipients.Add(new Recipient
                {
                    EmailAddress = new EmailAddress { Address = recipientEmail }
                });
                foreach (string cc in emailParams.Cc)
                {
                    message.CcRecipients.Add(new Recipient
                    {
                        EmailAddress = new EmailAddress { Address = cc }
                    });
                }
                foreach (string bcc in emailParams.Bcc)
                {
                    message.BccRecipients.Add(new Recipient
                    {
                        EmailAddress = new EmailAddress { Address = bcc }
                    });
                }
                var ccEmails = string.IsNullOrWhiteSpace(settings.CcEmail) ? [] : settings.CcEmail.Split(",");
                foreach (string ccEmail in ccEmails)
                {
                    message.BccRecipients.Add(new Recipient
                    {
                        EmailAddress = new EmailAddress { Address = ccEmail.Trim() }
                    });
                }
            }
            foreach (var attachment in emailParams.Attachments)
            {
                byte[] fileBytes;
                if (attachment.IsPath)
                {
                    fileBytes = await File.ReadAllBytesAsync(attachment.FilePath);
                }
                else
                {
                    using var memoryStream = new MemoryStream();
                    await attachment.ContentStream.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }
                var fileAttachment = new FileAttachment
                {
                    Name = attachment.Name,
                    ContentBytes = fileBytes,
                    ContentType = attachment.MediaType
                };
                message.Attachments.Add(fileAttachment);
            }
            var userEmail = _graphClient.Users[settings.FromEmail];
            var emailBuilder = userEmail.SendMail;
            await emailBuilder.PostAsync(new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = true
            });

            _logger.LogInformation("Email sent successfully via Microsoft 365 OAuth to {Recipient} using {EmailSettingName}",
                emailParams.Email, emailParams.EmailSettingName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR SENDING EMAIL VIA MICROSOFT 365: {Message}", ex.Message);
            return false;
        }
    }
}