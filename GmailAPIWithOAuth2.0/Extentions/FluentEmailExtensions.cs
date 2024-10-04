using FluentEmail.Core;

namespace GmailAPIWithOAuth2.Extentions
{
    public static class FluentEmailExtensions
    {
        public static IFluentEmail From(this IFluentEmail fluentEmail, string emailAddress, string name = null)
        {
            fluentEmail.SetFrom(emailAddress, name);

            return fluentEmail;
        }
    }
}
