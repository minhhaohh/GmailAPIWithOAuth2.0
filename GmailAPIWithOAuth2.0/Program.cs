using Fluid;
using GmailAPIWithOAuth2.Extentions;
using GmailAPIWithOAuth2.Models;
using GmailAPIWithOAuth2.Services.ReadEmails;
using GmailAPIWithOAuth2.Services.SendEmails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GmailAPIWithOAuth2
{
    class Program
    {
        public static IConfiguration Configuration;

        private static async Task Main(string[] args)
        {
            var services = ConfigureServices();

            try
            {
                var serviceProvider = services.BuildServiceProvider();

                var sendMailFactory = serviceProvider.GetRequiredService<ISendMailServiceFactory>();
                var readMailFactory = serviceProvider.GetRequiredService<IReadMailServiceFactory>();

                // Using Gmail Smtp
                var gmailSmtpOptions = serviceProvider.GetRequiredService<IOptions<TestGmailSmtpOptions>>().Value;
                var gmailSmtpService = sendMailFactory.CreateMailService(gmailSmtpOptions.MailServiceName);

                await gmailSmtpService
                    .From(gmailSmtpOptions.SenderAddress, gmailSmtpOptions.SenderName)
                    .To(gmailSmtpOptions.ReceiverAddress, gmailSmtpOptions.ReceiverName)
                    .Subject(gmailSmtpOptions.Subject)
                    .Body("Test Gmail Smtp")
                    .SendAsync();

                // Using Gmail Smtp and authenticating with OAuth2.0
                var oAuth2GmailSmtpOptions = serviceProvider.GetRequiredService<IOptions<TestOAuth2GmailSmtpOptions>>().Value;
                var oAuth2GmailSmptService = sendMailFactory.CreateMailService(oAuth2GmailSmtpOptions.MailServiceName);
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "TestTemplate.liquid");
                var order = new Order
                {
                    OrderNumber = oAuth2GmailSmtpOptions.MailServiceName,
                    Customer = new Person()
                    {
                        Name = "Hao Tran",
                        Dob = new DateTime(2000, 12, 4),
                        Gender = "Male",
                        Address = "Ho Chi Minh city"
                    },
                    Products = new List<Product>
                    {
                        new Product { Name = "Laptop", Price = 999.99m, Quantity = 1 },
                        new Product { Name = "Mouse", Price = 49.99m, Quantity = 2 },
                        new Product { Name = "Keyboard", Price = 79.99m, Quantity = 1 }
                    }
                };

                var response = await oAuth2GmailSmptService
                    .From(oAuth2GmailSmtpOptions.SenderAddress, oAuth2GmailSmtpOptions.SenderName)
                    .To(oAuth2GmailSmtpOptions.ReceiverAddress, oAuth2GmailSmtpOptions.ReceiverName)
                    .Subject(oAuth2GmailSmtpOptions.Subject)
                    .UsingTemplateFromFile(templatePath, order)
                    .SendAsync();


                // Using Send Grid Api
                var sendGridApiOptions = serviceProvider.GetRequiredService<IOptions<TestSendGridApiOptions>>().Value;
                var sendGridMailService = sendMailFactory.CreateMailService(sendGridApiOptions.MailServiceName);

                await sendGridMailService
                    .From(sendGridApiOptions.SenderAddress, sendGridApiOptions.SenderName)
                    .To(sendGridApiOptions.ReceiverAddress, sendGridApiOptions.ReceiverName)
                    .Subject(sendGridApiOptions.Subject)
                    .Body("Test Send Grid Api")
                    .SendAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static ServiceCollection ConfigureServices()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.mailing.json", false)
                .Build();

            var services = new ServiceCollection();

            services.Configure<MailOptions>(Configuration.GetSection(nameof(MailOptions)));

            services.Configure<TestGmailSmtpOptions>(Configuration.GetSection(nameof(TestGmailSmtpOptions)));
            services.Configure<TestOAuth2GmailSmtpOptions>(Configuration.GetSection(nameof(TestOAuth2GmailSmtpOptions)));
            services.Configure<TestSendGridApiOptions>(Configuration.GetSection(nameof(TestSendGridApiOptions)));

            services.AddSingleton<ISendMailServiceFactory, SendMailServiceFactory>();
            services.AddSingleton<IReadMailServiceFactory, ReadMailServiceFactory>();

            var mailOptions = Configuration.GetSection(nameof(MailOptions)).Get<MailOptions>();

            // Add Fluent Email
            services.AddFluentEmail(mailOptions.DefaultFromEmail)
                .AddLiquidRenderer(options =>
                {
                    // To use with complex model
                    options.TemplateOptions.MemberAccessStrategy = UnsafeMemberAccessStrategy.Instance;
                });

            return services;
        }
    }
}