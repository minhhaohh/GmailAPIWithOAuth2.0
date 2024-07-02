using GmailAPIWithOAuth2.Models;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
    public interface ISendMailServiceFactory
    {
        ISendMailService Create();

        ISendMailService CreateSmtpClient(SmtpContext smtpContext = null);
    }
}
