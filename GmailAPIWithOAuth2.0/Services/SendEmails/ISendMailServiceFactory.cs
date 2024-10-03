using GmailAPIWithOAuth2.Models;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
    public interface ISendMailServiceFactory
    {
        ISendMailService CreateSendMailService();

        ISendMailService CreateSmtpMailService(SmtpContext smtpContext = null);

        ISendMailService CreateFluentMailService();
    }
}
