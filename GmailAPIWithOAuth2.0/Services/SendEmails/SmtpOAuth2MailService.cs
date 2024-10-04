using GmailAPIWithOAuth2.Models;
using Google.Apis.Auth.OAuth2;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
    public class SmtpOAuth2MailService : ISendMailService
    {
        OAuth2SmtpContext _context;
        MimeMessage _currentMessage;
        BodyBuilder _bodyBuilder;

        public SmtpOAuth2MailService(OAuth2SmtpContext context)
        {
            _context = context;
        }

		public virtual void Send()
        {
            if (_currentMessage == null)
                throw new NullReferenceException("Mail message is not initialized.");

            if (_bodyBuilder != null)
                _currentMessage.Body = _bodyBuilder.ToMessageBody();

            using (var client = CreateSmtpClient())
            {
                client.Send(_currentMessage);
                client.Disconnect(true);
            }

            _currentMessage = null;
            _bodyBuilder = null;
        }

		public virtual async Task SendAsync()
        {
            if (_currentMessage == null)
                throw new NullReferenceException("Mail message is not initialized.");

            if (_bodyBuilder != null)
                _currentMessage.Body = _bodyBuilder.ToMessageBody();

            using (var client = CreateSmtpClient())
            {
                await client.SendAsync(_currentMessage);
                client.Disconnect(true);
            }

            _currentMessage = null;
            _bodyBuilder = null;
        }

		public virtual ISendMailService New()
        {
            _currentMessage = new MimeMessage();
            _bodyBuilder = new BodyBuilder();

            return this;
        }

		public virtual ISendMailService Subject(string subject)
        {
            _currentMessage.Subject = subject;
            return this;
        }

		public virtual ISendMailService From(string senderAddress, string senderName = null)
        {
            _currentMessage.From.Add(new MailboxAddress(senderName, senderAddress));
            return this;
        }

		public virtual ISendMailService To(string address, string displayName = null)
        {
            _currentMessage.To.Add(new MailboxAddress(displayName, address));
            return this;
        }

		public virtual ISendMailService Cc(string address, string displayName = null)
        {
            _currentMessage.Cc.Add(new MailboxAddress(displayName, address));
            return this;
        }

		public virtual ISendMailService Bcc(string address, string displayName = null)
        {
            _currentMessage.Bcc.Add(new MailboxAddress(displayName, address));
            return this;
        }

		public virtual ISendMailService ReplyTo(string address, string displayName = null)
        {
            _currentMessage.ReplyTo.Add(new MailboxAddress(displayName, address));
            return this;
        }

        public virtual ISendMailService Body(string bodyContent, bool isHtml = false)
        {
            if (isHtml) 
                _bodyBuilder.HtmlBody = bodyContent;
            else
                _bodyBuilder.TextBody = bodyContent;
            
            return this;
        }

		public virtual ISendMailService BodyFromFile(string filePath, bool isHtml = false)
		{
			StreamReader sr;

			if (filePath.ToLower().StartsWith("http"))
			{
				var client = new HttpClient();
				sr = new StreamReader(client.GetStreamAsync(filePath).Result);
			}
			else
			{
				sr = new StreamReader(filePath, Encoding.Default);
			}

            if (isHtml)
                _bodyBuilder.HtmlBody = sr.ReadToEnd();
            else
                _bodyBuilder.TextBody = sr.ReadToEnd();

			sr.Close();

			return this;
		}

		public virtual ISendMailService Header(string key, string value)
        {
            _currentMessage.Headers.Add(key, value);
            return this;
        }

        public virtual ISendMailService AddAttachment(string filePath, string name = null)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return this;

            var fileName = string.IsNullOrWhiteSpace(name) ? Path.GetFileName(filePath) : name;
            var fileAsBytes = File.ReadAllBytes(filePath);

            _bodyBuilder.Attachments.Add(fileName, fileAsBytes);

            return this;
        }

        public virtual ISendMailService UsingTemplate<T>(string template, T model, bool isHtml = true)
        {
            return this;
        }

        public virtual ISendMailService UsingTemplateFromFile<T>(string filePath, T model, bool isHtml = true)
        {
            return this;
        }

        private SmtpClient CreateSmtpClient()
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
