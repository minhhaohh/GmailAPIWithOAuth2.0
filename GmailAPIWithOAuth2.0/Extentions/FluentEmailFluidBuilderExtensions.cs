using FluentEmail.Core.Interfaces;
using GmailAPIWithOAuth2.Models;
using GmailAPIWithOAuth2.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GmailAPIWithOAuth2.Extentions
{
    public static class FluentEmailFluidBuilderExtensions
    {
        public static FluentEmailServicesBuilder AddCustomLiquidRenderer(
            this FluentEmailServicesBuilder builder,
            Action<CustomLiquidRendererOptions> configure = null)
        {
            builder.Services.AddOptions<CustomLiquidRendererOptions>();
            if (configure != null)
            {
                builder.Services.Configure(configure);
            }

            builder.Services.TryAddSingleton<ITemplateRenderer, CustomLiquidRenderer>();
            return builder;
        }
    }
}
