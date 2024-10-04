using FluentEmail.Core;
using FluentEmail.SendGrid;
using FluentEmail.Smtp;
using GmailAPIWithOAuth2.Constants;
using GmailAPIWithOAuth2.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
    public class SendMailServiceFactory : ISendMailServiceFactory
    {
        private readonly MailOptions _mailOptions;
        private readonly IFluentEmail _fluentEmail;

        public SendMailServiceFactory(
            IOptions<MailOptions> mailOptions, 
            IFluentEmail fluentEmail)
        {
            _mailOptions = mailOptions.Value;
            _fluentEmail = fluentEmail;
        }

        public IFluentEmail CreateMailService(string serviceName)
        {
            return serviceName switch
            {
                MailServiceName.SendGridApi => CreateSendGridMailService(_mailOptions.SendGridApi),
                MailServiceName.OAuth2GmailSmtp => CreateOAuth2SmtpMailService(_mailOptions.OAuth2GmailSmtp),
                _ => CreateSmtpMailService(_mailOptions.GmailSmtp)
            };
        }

        public IFluentEmail CreateSmtpMailService(MailingContext context)
        {
            var smptClient = new SmtpClient(context.Host, context.Port)
            {
                EnableSsl = context.EnableSsl,
                Credentials = new NetworkCredential(context.Username, context.Password)
            };

            _fluentEmail.Sender = new SmtpSender(smptClient);

            return _fluentEmail;
        }

        public IFluentEmail CreateOAuth2SmtpMailService(MailingContext context)
        {
            _fluentEmail.Sender = new OAuth2MailkitSender(context);

            return _fluentEmail;
        }

        public IFluentEmail CreateSendGridMailService(ApiKey apiKey)
        {
            _fluentEmail.Sender = new SendGridSender(apiKey.Value);

            // Workaround 
            //_fluentEmail.Tag("string");

            return _fluentEmail;
        }
    }
}
