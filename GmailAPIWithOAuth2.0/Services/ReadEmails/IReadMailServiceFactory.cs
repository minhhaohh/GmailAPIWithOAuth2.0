using GmailAPIWithOAuth2.Models;

namespace GmailAPIWithOAuth2.Services.ReadEmails
{
    public interface IReadMailServiceFactory
    {
		IReadMailService CreateReadMailService();

		IReadMailService CreateImapMailService(MailingContext context = null);
    }
}
