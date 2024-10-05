using Telegram.Bot.Types;

namespace Birthday.Bot.Abstracts;

public interface IHandler
{
	Task RunAsync(Update update, CancellationToken cancellationToken);
}