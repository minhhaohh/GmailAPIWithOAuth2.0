using MailKit;

namespace GmailAPIWithOAuth2.Services.ReadEmails
{
	public interface IReadMailService
	{
		List<UniqueId> GetEmailIds(DateTime? fromDate = null, bool notSeenOnly = false);

		Task<List<UniqueId>> GetEmailIdsAsync(DateTime? fromDate = null, bool notSeenOnly = false);
	}
}
