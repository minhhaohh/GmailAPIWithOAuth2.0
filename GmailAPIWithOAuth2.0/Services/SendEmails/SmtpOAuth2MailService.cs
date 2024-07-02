using MimeKit;
using GmailAPIWithOAuth2.Models;

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

        public void Send()
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

        public async Task SendAsync()
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

        public ISendMailService New()
        {
            _currentMessage = new MimeMessage();

            return this;
        }

        public ISendMailService Subject(string subject)
        {
            _currentMessage.Subject = subject;
            return this;
        }

        public ISendMailService From(string senderAddress, string senderName = null)
        {
            _currentMessage.From.Add(new MailboxAddress(senderName, senderAddress));
            return this;
        }

        public ISendMailService To(string address, string displayName = null)
        {
            _currentMessage.To.Add(new MailboxAddress(displayName, address));
            return this;
        }

        public ISendMailService Cc(string address, string displayName = null)
        {
            _currentMessage.Cc.Add(new MailboxAddress(displayName, address));
            return this;
        }

        public ISendMailService Bcc(string address, string displayName = null)
        {
            _currentMessage.Bcc.Add(new MailboxAddress(displayName, address));
            return this;
        }

        public ISendMailService ReplyTo(string address, string displayName = null)
        {
            _currentMessage.ReplyTo.Add(new MailboxAddress(displayName, address));
            return this;
        }

        public ISendMailService Body(string bodyContent)
        {
            _currentMessage.Body = new TextPart("plain") { Text = bodyContent };
            return this;
        }

        public ISendMailService Header(string key, string value)
        {
            _currentMessage.Headers.Add(key, value);
            return this;
        }
    }
}
