using GmailAPIWithOAuth2.Extentions;
using GmailAPIWithOAuth2.Models;
using GmailAPIWithOAuth2.Services.ReadEmails;
using GmailAPIWithOAuth2.Services.SendEmails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

				var sendMailService = sendMailFactory.CreateSendMailService();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "TestTemplate.liquid");
                var model = new Person()
                {
                    Name = "Hao Tran",
                    Dob = new DateTime(2000, 12, 4),
                    Gender = "Male",
                    Address = "Ho Chi Minh city"
                };
                await sendMailService.New()
					.From("haotrandevsoft@gmail.com", "Hao Tran")
					.To("hao.tran@devsoft.vn", "Minh Hao")
					.Subject("TEST EMAIL SUBJECT")
					.UsingTemplateFromFile(filePath, model)
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
				.AddJsonFile("appsettings.mailing.json", false)
				.Build();

			var services = new ServiceCollection();

			services.Configure<MailOptions>(Configuration.GetSection(nameof(MailOptions)));

			services.AddSingleton<ISendMailServiceFactory, SendMailServiceFactory>();
			services.AddSingleton<IReadMailServiceFactory, ReadMailServiceFactory>();

			var mailOptions = Configuration.GetSection(nameof(MailOptions)).Get<MailOptions>();

            // Add Fluent Email
            services.AddFluentEmail(mailOptions.GmailSmtp.Username)
				.AddOAuth2MailKitSender(mailOptions.GmailSmtp)
				.AddLiquidRenderer();

			return services;
		}
	}
}