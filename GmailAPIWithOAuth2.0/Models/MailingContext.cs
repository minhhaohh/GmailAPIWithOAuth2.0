namespace GmailAPIWithOAuth2.Models
{
    public class MailingContext
	{
		public string Host { get; set; }

		public int Port { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public bool EnableSsl { get; set; }
	}
}
