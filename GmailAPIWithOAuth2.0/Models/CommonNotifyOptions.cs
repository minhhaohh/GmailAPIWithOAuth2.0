namespace GmailAPIWithOAuth2.Models
{
    public class CommonNotifyOptions
    {
        public string Subject { get; set; }

        public string MailServiceName { get; set; }

        public string SenderName { get; set; }

        public string SenderAddress { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverAddress { get; set; }
    }

    public class TestGmailSmtpOptions : CommonNotifyOptions
    {

    }

    public class TestOAuth2GmailSmtpOptions : CommonNotifyOptions
    {

    }

    public class TestSendGridApiOptions : CommonNotifyOptions
    {

    }
}
