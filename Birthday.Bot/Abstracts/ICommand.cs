using Telegram.Bot.Types;

namespace Birthday.Bot.Abstracts;

public interface ICommand
{
	Task RunAsync(Message message, CancellationToken cancellationToken);
}