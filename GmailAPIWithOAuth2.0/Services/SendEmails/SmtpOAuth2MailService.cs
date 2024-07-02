using GmailAPIWithOAuth2.Models;
using MimeKit;
using System.Text;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
	public class SmtpOAuth2MailService : ISendMailService
    {
        SmtpContext _context;
        MimeMessage _currentMessage;

        public SmtpOAuth2MailService(SmtpContext context)
        {
            _context = context;
        }

		public virtual void Send()
        {
            if (_currentMessage == null)
                throw new NullReferenceException("Mail message is not initialized.");

            using (var client = _context.CreateSmtpClient())
            {
                client.Send(_currentMessage);
                client.Disconnect(true);
            }

            _currentMessage = null;
        }

		public virtual async Task SendAsync()
        {
            if (_currentMessage == null)
                throw new NullReferenceException("Mail message is not initialized.");

            using (var client = _context.CreateSmtpClient())
            {
                await client.SendAsync(_currentMessage);
                client.Disconnect(true);
            }

            _currentMessage = null;
        }

		public virtual ISendMailService New()
        {
            _currentMessage = new MimeMessage();

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
			_currentMessage.Body = isHtml
                ? new TextPart("html") { Text = bodyContent }
                : new TextPart("plain") { Text = bodyContent };
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

			_currentMessage.Body = new TextPart("html") { Text = sr.ReadToEnd() };
			sr.Close();

			return this;
		}

		public virtual ISendMailService Header(string key, string value)
        {
            _currentMessage.Headers.Add(key, value);
            return this;
        }
    }
}
