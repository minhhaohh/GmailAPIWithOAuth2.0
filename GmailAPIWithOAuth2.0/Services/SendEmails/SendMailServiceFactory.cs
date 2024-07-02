using Microsoft.Extensions.Options;
using GmailAPIWithOAuth2.Models;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
    public class SendMailServiceFactory : ISendMailServiceFactory
    {
        private readonly MailOptions _mailOptions;

        public SendMailServiceFactory(IOptions<MailOptions> mailOptions)
        {
            _mailOptions = mailOptions.Value;
        }

        public ISendMailService Create()
        {
            return CreateSmtpClient(_mailOptions.GmailSmtp);
        }

        public ISendMailService CreateSmtpClient(SmtpContext smtpContext = null)
        {
            return new SmtpOAuth2MailService(smtpContext);
        }
    }
}
