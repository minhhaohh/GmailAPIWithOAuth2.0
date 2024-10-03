using Microsoft.Extensions.Options;
using GmailAPIWithOAuth2.Models;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
    public class SendMailServiceFactory : ISendMailServiceFactory
    {
        private readonly MailOptions _mailOptions;
        private readonly IFluentEmail _fluentEmail;

        public SendMailServiceFactory(
            IOptions<MailOptions> mailOptions,
            IServiceProvider serviceProvider)
        {
            _mailOptions = mailOptions.Value;
            _fluentEmail = serviceProvider.GetService<IFluentEmail>();
        }

        public ISendMailService CreateSendMailService()
        {
            return _fluentEmail == null
                ? CreateSmtpMailService(_mailOptions.GmailSmtp)
                : CreateFluentMailService();
        }

        public ISendMailService CreateSmtpMailService(SmtpContext smtpContext = null)
        {
            return new SmtpOAuth2MailService(smtpContext);
        }

        public ISendMailService CreateFluentMailService()
        {
            return new FluentMailService(_fluentEmail);
        }
    }
}
