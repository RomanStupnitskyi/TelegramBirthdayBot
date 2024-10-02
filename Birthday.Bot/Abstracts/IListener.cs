using Telegram.Bot.Types;

namespace Birthday.Bot.Abstracts;

public interface IListener
{
	Task RunAsync(Update update, CancellationToken cancellationToken);
}