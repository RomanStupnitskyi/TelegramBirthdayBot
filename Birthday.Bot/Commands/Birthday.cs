using Birthday.Bot.Abstracts;
using Birthday.Bot.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Birthday.Bot.Commands;

[Command("birthday", "Set your birthday")]
public class Birthday(ITelegramBotClient telegramBotClient) : ICommand
{
	public async Task RunAsync(Message message, CancellationToken cancellationToken)
	{
		await telegramBotClient.SendTextMessageAsync(
			message.Chat.Id,
				"Вкажи свій день народження у форматі ДД.ММ.РРРР",
			cancellationToken: cancellationToken);
	}
}