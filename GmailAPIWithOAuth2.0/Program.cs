using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GmailAPIWithOAuth2.Models;
using GmailAPIWithOAuth2.Services.ReadEmails;
using GmailAPIWithOAuth2.Services.SendEmails;
using DotNetEnv;

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

				var sendMailService = sendMailFactory.Create();

				await sendMailService.New()
					.From("minhhao.hh00@gmail.com", "Minh Hao")
					.To("hao.tran@devsoft.vn", "Hao Tran")
					.Subject("TEST EMAIL SUBJECT")
					.Body("TEST EMAIL BODY")
					.SendAsync();

				var readMailService = readMailFactory.Create();
				await readMailService.GetEmailIdsAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
		}

		private static ServiceCollection ConfigureServices()
		{
			// Load environment variables from .env
			Env.Load();

			// Get values from environment variables
			var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
			var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");

			Configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.mailing.json", false)
				.Build();

			// Update values from environment variables to configuration
			Configuration["MailOptions:GmailSmtp:Username"] = clientId;
			Configuration["MailOptions:GmailSmtp:Password"] = clientSecret;
			Configuration["MailOptions:GmailImap:Username"] = clientId;
			Configuration["MailOptions:GmailImap:Password"] = clientSecret;

			var services = new ServiceCollection();

			services.Configure<MailOptions>(Configuration.GetSection(nameof(MailOptions)));

			services.AddSingleton<ISendMailServiceFactory, SendMailServiceFactory>();
			services.AddSingleton<IReadMailServiceFactory, ReadMailServiceFactory>();

			return services;
		}
	}
}