using GmailAPIWithOAuth2.Models;
using Microsoft.Extensions.Options;

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
        public IReadMailService CreateImapMailService(MailingContext context = null)
        {
            return new ImapOAuth2MailService(context);
        }
    }
}
