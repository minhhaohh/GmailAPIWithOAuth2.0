using FluentEmail.Core.Interfaces;
using GmailAPIWithOAuth2.Models;
using GmailAPIWithOAuth2.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GmailAPIWithOAuth2.Extentions
{
    public static class FluentEmailExtensions
    {
        public static FluentEmailServicesBuilder AddOAuth2MailKitSender(this FluentEmailServicesBuilder builder, SmtpContext smtpContext)
        {
            builder.Services.TryAdd(
                ServiceDescriptor.Scoped((Func<IServiceProvider, ISender>)((IServiceProvider _) 
                    => new OAuth2MailkitSender(smtpContext))));
            return builder;
        }
    }
}
