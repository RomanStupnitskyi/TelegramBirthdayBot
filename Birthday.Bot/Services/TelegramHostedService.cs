using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Birthday.Bot.Services;

public class TelegramHostedService(
	ITelegramBotClient botClient,
	ITelegramBotService telegramBotService,
	MongoDatabaseService mongoDatabaseService) : IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await mongoDatabaseService.SessionStateProvider.EnsureIndexesAsync();
		// await telegramBotService.UpdateBotCommandsAsync(cancellationToken); // It works btw
		
		botClient.StartReceiving(
			HandleUpdateAsync,
			HandleErrorAsync,
			null,
			cancellationToken);
	}
	
	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update,
		CancellationToken cancellationToken)
	{
		await telegramBotService.HandleUpdateAsync(update, cancellationToken);
	}
	
	private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
		CancellationToken cancellationToken)
	{
		var errorMessage = exception switch
		{
			ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
			_ => exception.ToString()
		};
		
		Console.WriteLine(errorMessage);
		return Task.CompletedTask;
	}
}