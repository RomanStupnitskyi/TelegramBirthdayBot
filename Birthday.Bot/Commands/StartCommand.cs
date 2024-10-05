using Birthday.Bot.Abstracts;
using Birthday.Bot.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Birthday.Bot.Commands;

[Command("start", "Start command")]
public class StartCommand(ITelegramBotClient telegramBotClient) : BaseCommand(telegramBotClient)
{
	private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;

	public override async Task RunAsync(Message message, CancellationToken cancellationToken)
	{
		await _telegramBotClient.SendTextMessageAsync(
			message.Chat.Id,
				"Привіт! Я надсилаю привітання з днем народження.\n\nЩоб я пригадував твій день народження також, вкажи його за допомогою команди /birthday",
			cancellationToken: cancellationToken);
	}
}