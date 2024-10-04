namespace GmailAPIWithOAuth2.Models
{
	public class MailOptions
	{
        public string DefaultFromEmail { get; set; }

        public SmtpContext GmailSmtp { get; set; }

        public OAuth2SmtpContext GmailOAuth2Smtp { get; set; }

        public ApiKey SendGridApi { get; set; }

        public ImapContext GmailImap { get; set; }
	}
}
