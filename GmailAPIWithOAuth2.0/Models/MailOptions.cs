namespace GmailAPIWithOAuth2.Models
{
	public class MailOptions
	{
		public SmtpContext GmailSmtp { get; set; }

		public ImapContext GmailImap { get; set; }
	}
}
