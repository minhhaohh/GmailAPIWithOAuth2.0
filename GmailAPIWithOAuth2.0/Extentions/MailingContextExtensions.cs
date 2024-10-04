using GmailAPIWithOAuth2.Models;
using Google.Apis.Auth.OAuth2;
using MailKit.Net.Imap;
using MailKit.Security;
using System.Net;
using System.Net.Mail;
using OAuthSmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace GmailAPIWithOAuth2.Extentions
{
    public static class MailingContextExtensions
    {
        public static SmtpClient CreateSmtpClient(this MailingContext context)
        {
            return new SmtpClient(context.Host, context.Port)
            {
                EnableSsl = context.EnableSsl,
                Credentials = new NetworkCredential(context.Username, context.Password)
            };
        }

        public static OAuthSmtpClient CreateOAuth2SmtpClient(this MailingContext context)
        {
            var smtpClient = new OAuthSmtpClient();

            smtpClient.Connect(context.Host, context.Port, context.EnableSsl);

            // Define the scope for Gmail
            var scopes = new[] { "https://mail.google.com/" };

            // Authorize and get credentials
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets() { ClientId = context.ClientId, ClientSecret = context.ClientSecret },
            scopes,
            context.Username,
                CancellationToken.None).Result;

            // Check if access token is expired and refresh if necessary
            if (credential.Token.IsStale)
            {
                credential.RefreshTokenAsync(CancellationToken.None).Wait();
            }

            // Create an OAuth2 authentication mechanism using the email address and the access token obtained from the Google credentials.
            var oauth2 = new SaslMechanismOAuth2(context.Username, credential.Token.AccessToken);
            smtpClient.Authenticate(oauth2);

            return smtpClient;
        }

        public static ImapClient CreateImapClient(this MailingContext context)
        {
            var imapClient = new ImapClient();

            imapClient.Connect(context.Host, context.Port, context.EnableSsl);

            // Define the scope for Gmail
            var scopes = new[] { "https://mail.google.com/" };

            // Authorize and get credentials
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets() { ClientId = context.ClientId, ClientSecret = context.ClientSecret },
                scopes,
                context.Username,
                CancellationToken.None).Result;

            // Check if access token is expired and refresh if necessary
            if (credential.Token.IsStale)
            {
                credential.RefreshTokenAsync(CancellationToken.None).Wait();
            }

            // Create an OAuth2 authentication mechanism using the email address and the access token obtained from the Google credentials.
            var oauth2 = new SaslMechanismOAuth2(context.Username, credential.Token.AccessToken);
            imapClient.Authenticate(oauth2);

            return imapClient;
        }
    }
}
