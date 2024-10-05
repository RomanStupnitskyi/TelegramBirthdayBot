using Telegram.Bot;
using Telegram.Bot.Types;

namespace Birthday.Bot.Abstracts;

public abstract class BaseCommand(ITelegramBotClient telegramBotClient) : ICommand
{
	public virtual Task RunAsync(Message message, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
	
	public virtual async Task NoPermissionAsync(Message message, CancellationToken cancellationToken)
	{
		await telegramBotClient.SendTextMessageAsync(
			message.Chat.Id,
			"\u274c Недостатньо прав для виконання цієї команди",
			cancellationToken: cancellationToken);
	}
	
	public virtual async Task NoAccessAsync(Message message, CancellationToken cancellationToken)
	{
		await telegramBotClient.SendTextMessageAsync(
			message.Chat.Id,
			"\u274c Ви не можете виконати цю команду тут",
			cancellationToken: cancellationToken);
	}
}