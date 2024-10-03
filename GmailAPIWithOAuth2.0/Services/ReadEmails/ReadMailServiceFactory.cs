using Microsoft.Extensions.Options;
using GmailAPIWithOAuth2.Models;

namespace GmailAPIWithOAuth2.Services.ReadEmails
{
    public class ReadMailServiceFactory : IReadMailServiceFactory
    {
        private readonly MailOptions _mailOptions;

        public ReadMailServiceFactory(IOptions<MailOptions> mailOptions)
        {
            _mailOptions = mailOptions.Value;
        }

        public IReadMailService CreateReadMailService()
        {
            return CreateImapMailService(_mailOptions.GmailImap);
        }
        public IReadMailService CreateImapMailService(ImapContext imapContext = null)
        {
            return new ImapOAuth2MailService(imapContext);
        }
    }
}
