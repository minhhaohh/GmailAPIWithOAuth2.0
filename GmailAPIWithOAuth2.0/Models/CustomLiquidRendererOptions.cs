using FluentEmail.Liquid;
using Fluid;

namespace GmailAPIWithOAuth2.Models
{
    public class CustomLiquidRendererOptions : LiquidRendererOptions
    {
        /// <summary>
        /// Set custom Template Options for Fluid 
        /// </summary>
        public TemplateOptions TemplateOptions { get; set; } = new TemplateOptions();
    }
}
