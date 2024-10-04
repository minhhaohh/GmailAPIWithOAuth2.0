using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using GmailAPIWithOAuth2.Models;
using Google.Apis.Auth.OAuth2;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text;

namespace GmailAPIWithOAuth2.Services
{
    public class OAuth2MailkitSender : ISender
    {
        private readonly OAuth2SmtpContext _context;

        public OAuth2MailkitSender(OAuth2SmtpContext context)
        {
            _context = context;
        }

        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            SendResponse sendResponse = new SendResponse();
            MimeMessage message = CreateMailMessage(email);
            if (token.HasValue && token.GetValueOrDefault().IsCancellationRequested)
            {
                sendResponse.ErrorMessages.Add("Message was cancelled by cancellation token.");
                return sendResponse;
            }

            try
            {
                using SmtpClient client = CreateOAuthSmtpClient();

                client.Send(message, token.GetValueOrDefault());
                client.Disconnect(quit: true, token.GetValueOrDefault());
            }
            catch (Exception ex)
            {
                sendResponse.ErrorMessages.Add(ex.Message);
            }

            return sendResponse;
        }

        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            SendResponse response = new SendResponse();
            MimeMessage message = CreateMailMessage(email);
            if (token?.IsCancellationRequested ?? false)
            {
                response.ErrorMessages.Add("Message was cancelled by cancellation token.");
                return response;
            }

            try
            {
                using SmtpClient client = CreateOAuthSmtpClient();

                await client.SendAsync(message, token.GetValueOrDefault());
                await client.DisconnectAsync(quit: true, token.GetValueOrDefault());
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
            }

            return response;
        }

        private MimeMessage CreateMailMessage(IFluentEmail email)
        {
            EmailData data = email.Data;
            MimeMessage message = new MimeMessage();
            if (!message.Headers.Contains(HeaderId.Subject))
            {
                message.Headers.Add(HeaderId.Subject, Encoding.UTF8, data.Subject ?? string.Empty);
            }
            else
            {
                message.Headers[HeaderId.Subject] = data.Subject ?? string.Empty;
            }

            message.Headers.Add(HeaderId.Encoding, Encoding.UTF8.EncodingName);
            message.From.Add(new MailboxAddress(data.FromAddress.Name, data.FromAddress.EmailAddress));
            BodyBuilder builder = new BodyBuilder();
            if (!string.IsNullOrEmpty(data.PlaintextAlternativeBody))
            {
                builder.TextBody = data.PlaintextAlternativeBody;
                builder.HtmlBody = data.Body;
            }
            else if (!data.IsHtml)
            {
                builder.TextBody = data.Body;
            }
            else
            {
                builder.HtmlBody = data.Body;
            }

            data.Attachments.ForEach(delegate (Attachment x)
            {
                MimeEntity mimeEntity = builder.Attachments.Add(x.Filename, x.Data, ContentType.Parse(x.ContentType));
                mimeEntity.ContentId = x.ContentId;
            });
            message.Body = builder.ToMessageBody();
            foreach (KeyValuePair<string, string> header in data.Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }

            data.ToAddresses.ForEach(delegate (Address x)
            {
                message.To.Add(new MailboxAddress(x.Name, x.EmailAddress));
            });
            data.CcAddresses.ForEach(delegate (Address x)
            {
                message.Cc.Add(new MailboxAddress(x.Name, x.EmailAddress));
            });
            data.BccAddresses.ForEach(delegate (Address x)
            {
                message.Bcc.Add(new MailboxAddress(x.Name, x.EmailAddress));
            });
            data.ReplyToAddresses.ForEach(delegate (Address x)
            {
                message.ReplyTo.Add(new MailboxAddress(x.Name, x.EmailAddress));
            });
            switch (data.Priority)
            {
                case Priority.Low:
                    message.Priority = MessagePriority.NonUrgent;
                    break;
                case Priority.Normal:
                    message.Priority = MessagePriority.Normal;
                    break;
                case Priority.High:
                    message.Priority = MessagePriority.Urgent;
                    break;
            }

            return message;
        }

        public SmtpClient CreateOAuthSmtpClient()
        {
            var smtpClient = new SmtpClient();

            smtpClient.Connect(_context.Host, _context.Port, _context.EnableSsl);

            // Define the scope for Gmail
            var scopes = new[] { "https://mail.google.com/" };

            // Authorize and get credentials
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets() { ClientId = _context.ClientId, ClientSecret = _context.ClientSecret },
                scopes,
                _context.Username,
                CancellationToken.None).Result;

            // Check if access token is expired and refresh if necessary
            if (credential.Token.IsStale)
            {
                credential.RefreshTokenAsync(CancellationToken.None).Wait();
            }

            // Create an OAuth2 authentication mechanism using the email address and the access token obtained from the Google credentials.
            var oauth2 = new SaslMechanismOAuth2(_context.Username, credential.Token.AccessToken);
            smtpClient.Authenticate(oauth2);

            return smtpClient;
        }
    }
}
