using GmailAPIWithOAuth2.Extentions;
using GmailAPIWithOAuth2.Models;
using MailKit;
using MailKit.Search;
using MimeKit;

namespace GmailAPIWithOAuth2.Services.ReadEmails
{
    public class ImapOAuth2MailService : IReadMailService
	{
        MailingContext _context;

		public ImapOAuth2MailService(MailingContext context)
        {
            _context = context;
        }

		public List<UniqueId> GetEmailIds(DateTime? fromDate = null, bool notSeenOnly = false)
        {
			var uids = new List<UniqueId>();
			using (var client = _context.CreateImapClient())
			{
				// Open the inbox folder
				var inbox = client.Inbox;
				inbox.Open(FolderAccess.ReadOnly);

				var query = SearchQuery.All;
				if (!fromDate.HasValue)
				{
					query.And(SearchQuery.DeliveredAfter(fromDate.Value));
				}

				if (notSeenOnly)
				{
					query.And(SearchQuery.NotSeen);
				}

				uids = inbox.Search(query).ToList();

				foreach (var uid in uids)
				{
					var message = inbox.GetMessage(uid);
					Console.WriteLine($"Subject: {message.Subject}");
					Console.WriteLine($"From: {string.Join(", ", message.From)}");
					Console.WriteLine($"Date: {message.Date}");
					// Get the plain text body
					var textBody = message.TextBody ?? message.HtmlBody ?? GetTextFromMimeParts(message.BodyParts);
					Console.WriteLine($"Body: {textBody}");
					Console.WriteLine();
				}

				// Disconnect from the IMAP server
				client.Disconnect(true);
			}
			return uids;
		}

		public async Task<List<UniqueId>> GetEmailIdsAsync(DateTime? fromDate = null, bool notSeenOnly = false)
		{
			var uids = new List<UniqueId>();
			using (var client = _context.CreateImapClient())
			{
				// Open the inbox folder
				var inbox = client.Inbox;
				await inbox.OpenAsync(FolderAccess.ReadOnly);

				var query = SearchQuery.All;
				if (fromDate.HasValue)
				{
					query.And(SearchQuery.DeliveredAfter(fromDate.Value));
				}

				if (notSeenOnly)
				{
					query.And(SearchQuery.NotSeen);
				}

				uids = (await inbox.SearchAsync(query)).ToList();

				foreach (var uid in uids)
				{
					var message = await inbox.GetMessageAsync(uid);
					Console.WriteLine($"Subject: {message.Subject}");
					Console.WriteLine($"From: {string.Join(", ", message.From)}");
					Console.WriteLine($"Date: {message.Date}");
					// Get the plain text body
					var textBody = message.TextBody ?? message.HtmlBody ?? GetTextFromMimeParts(message.BodyParts);
					Console.WriteLine($"Body: {textBody}");
					Console.WriteLine();
				}

				// Disconnect from the IMAP server
				await client.DisconnectAsync(true);
			}
			return uids;
		}

		private string GetTextFromMimeParts(IEnumerable<MimeEntity> parts)
		{
			foreach (var part in parts)
			{
				if (part is TextPart textPart)
				{
					return textPart.Text;
				}
				else if (part is Multipart multipart)
				{
					var text = GetTextFromMimeParts(multipart);
					if (!string.IsNullOrEmpty(text))
						return text;
				}
			}
			return null;
		}
	}
}
