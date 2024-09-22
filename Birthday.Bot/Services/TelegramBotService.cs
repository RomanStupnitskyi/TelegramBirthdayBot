using Telegram.Bot;
using Telegram.Bot.Types;

namespace Birthday.Bot.Services;

public interface ITelegramBotService
{
	Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);	
}

public class TelegramBotService(ITelegramBotClient _botClient) : ITelegramBotService
{
	public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
	{
		var message = update.Message;

		if (message?.Text != null)
		{
			await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"You said '{message.Text}'"
, cancellationToken: cancellationToken);
		}
	}
}