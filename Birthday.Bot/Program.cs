using Birthday.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Birthday.Bot;

internal static class Program
{
	private static void Main(string[] args)
	{
		CreateHostBuilder(args).Build().Run();
	}

	private static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.UseEnvironment(Environment.GetEnvironmentVariable("ENV_NAME") ?? Environments.Production)
			.ConfigureAppConfiguration((context, config) =>
			{
				var environmentName = context.HostingEnvironment.EnvironmentName;
				config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
			})
			.ConfigureServices((hostContext, services) =>
			{
				var configuration = hostContext.Configuration;
				var telegramProperties = configuration.GetSection("TelegramProperties").Get<TelegramProperties>();

				services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(telegramProperties!.Token));
				services.AddSingleton<ITelegramBotService, TelegramBotService>();
				services.AddHostedService<TelegramHostedService>();
			});
}

internal class TelegramProperties
{
	public required string Token { get; init; }
}