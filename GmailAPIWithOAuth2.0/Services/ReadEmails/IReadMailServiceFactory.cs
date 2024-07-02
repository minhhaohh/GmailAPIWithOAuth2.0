using GmailAPIWithOAuth2.Models;

namespace GmailAPIWithOAuth2.Services.ReadEmails
{
    public interface IReadMailServiceFactory
    {
		IReadMailService Create();

		IReadMailService CreateImapClient(ImapContext imapContext = null);
    }
}
