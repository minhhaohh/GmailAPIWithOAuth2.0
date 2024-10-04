using FluentEmail.Core;
using GmailAPIWithOAuth2.Models;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
    public interface ISendMailServiceFactory
    {
        IFluentEmail CreateMailService(string serviceName);

        IFluentEmail CreateSmtpMailService(MailingContext context);

        IFluentEmail CreateOAuth2SmtpMailService(MailingContext context);

        IFluentEmail CreateSendGridMailService(ApiKey apiKey);
    }
}
