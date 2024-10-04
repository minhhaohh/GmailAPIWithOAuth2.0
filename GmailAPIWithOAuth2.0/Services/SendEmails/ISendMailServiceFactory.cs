using FluentEmail.Core;
using GmailAPIWithOAuth2.Models;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
    public interface ISendMailServiceFactory
    {
        IFluentEmail CreateMailService(string serviceName);

        IFluentEmail CreateSmtpMailService(SmtpContext smtpContext);

        IFluentEmail CreateOAuth2SmtpMailService(OAuth2SmtpContext context);

        IFluentEmail CreateSendGridMailService(ApiKey apiKey);
    }
}
