using Birthday.Bot.Abstracts;
using Birthday.Bot.Attributes;
using Birthday.Bot.MongoProviders;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Birthday.Bot.Commands;

[Command("show_birthday", "Show your birthday or replied user's birthday")]
public class ShowBirthdayCommand(ITelegramBotClient telegramBotClient, UserProvider userProvider) : ICommand
{
	public async Task RunAsync(Message message, CancellationToken cancellationToken)
	{
		var chatId = message.Chat.Id;
		if (message.From != null)
		{
			var userId = message.ReplyToMessage?.From?.Id ?? message.From.Id;
			var user = await userProvider.GetUserByIdAsync(userId);
		
			if (user == null)
			{
				await telegramBotClient.SendTextMessageAsync(
					chatId,
					"\ud83e\udd37\u200d\u2642\ufe0f Користувач ще не вказав свій день народження",
					cancellationToken: cancellationToken);
			}
			else
			{
				if (message.ReplyToMessage?.From?.Id != null)
				{
					var telegramUser = message.ReplyToMessage.From;
					await telegramBotClient.SendTextMessageAsync(
						chatId,
						$"\ud83c\udf89 День народження <b>{telegramUser.Username}</b>: {user.Birthday:dd.MM.yyyy}",
						cancellationToken: cancellationToken);
					return;
				}
				await telegramBotClient.SendTextMessageAsync(
					chatId,
					$"\ud83c\udf89 Твоє день народження: <b>{user.Birthday:dd.MM.yyyy}</b>",
					parseMode: ParseMode.Html,
					cancellationToken: cancellationToken);
			}
		}
	}
}