using Birthday.Bot.Abstracts;
using Birthday.Bot.Attributes;
using Telegram.Bot.Types;
using System.Reflection;

namespace Birthday.Bot.Listeners;

[Listener("Message")]
public class MessageListener(List<ICommand> commands) : IListener
{
	public async Task RunAsync(Update update, CancellationToken cancellationToken)
	{
		var message = update.Message;
		if (message?.Text != null)
		{
			if (message.Text.StartsWith('/'))
			{
				await HandleCommandAsync(update, cancellationToken);
			}
		}
	}
	
	private async Task HandleCommandAsync(Update update, CancellationToken cancellationToken)
	{
		var message = update.Message;
		var commandName = message?.Text?.Split(' ').First()[1..] ?? string.Empty;
		
		var command = commands.Find(x => x.GetType().GetCustomAttribute<CommandAttribute>()?.Name == commandName);
		if (message != null && command != null) await command.RunAsync(message, cancellationToken);
	}
}