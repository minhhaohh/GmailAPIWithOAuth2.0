using Google.Apis.Auth.OAuth2;
using MailKit.Net.Imap;
using MailKit.Security;

namespace GmailAPIWithOAuth2.Models
{
	public class ImapContext
	{
		public string Host { get; set; }

		public int Port { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public string Email { get; set; }

		public bool EnableSsl { get; set; }

		public ImapClient CreateImapClient()
		{
			var imapClient = new ImapClient();

			imapClient.Connect(Host, Port, EnableSsl);

			// Define the scope for Gmail
			var scopes = new[] { "https://mail.google.com/" };

			// Authorize and get credentials
			var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
				new ClientSecrets() { ClientId = Username, ClientSecret = Password },
				scopes,
				Email,
				CancellationToken.None).Result;

			// Check if access token is expired and refresh if necessary
			if (credential.Token.IsStale)
			{
				credential.RefreshTokenAsync(CancellationToken.None).Wait();
			}

			// Create an OAuth2 authentication mechanism using the email address and the access token obtained from the Google credentials.
			var oauth2 = new SaslMechanismOAuth2(Email, credential.Token.AccessToken);
			imapClient.Authenticate(oauth2);

			return imapClient;
		}
	}
}
