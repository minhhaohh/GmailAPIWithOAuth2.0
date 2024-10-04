namespace GmailAPIWithOAuth2.Models
{
	public class MailOptions
	{
        public string DefaultFromEmail { get; set; }

        public MailingContext GmailSmtp { get; set; }

        public MailingContext OAuth2GmailSmtp { get; set; }

        public ApiKey SendGridApi { get; set; }

        public MailingContext GmailImap { get; set; }
	}
}
