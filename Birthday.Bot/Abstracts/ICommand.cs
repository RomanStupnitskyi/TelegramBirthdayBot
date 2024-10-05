using Telegram.Bot.Types;

namespace Birthday.Bot.Abstracts;

public interface ICommand
{
	Task RunAsync(Message message, CancellationToken cancellationToken);
	Task NoPermissionAsync(Message message, CancellationToken cancellationToken);
	Task NoAccessAsync(Message message, CancellationToken cancellationToken);
}