using Birthday.Bot.Abstracts;
using Birthday.Bot.Attributes;
using Telegram.Bot.Types;
using System.Reflection;
using Birthday.Bot.MongoProviders;

namespace Birthday.Bot.Handlers;

[Handler("SessionHandler")]
public class SessionHandler(List<ICommand> commands, SessionStateProvider sessionStateProvider) : IHandler
{
	public async Task RunAsync(Update update, CancellationToken cancellationToken)
	{
		var message = update.Message;
		if (message?.Text != null)
		{
			if (message.Text.StartsWith('/'))
			{
				return;
			}
			
			var chatId = message.Chat.Id;
			var session = await sessionStateProvider.GetSessionStateByIdAsync(chatId);

			if (session != null)
			{
				session.State = message.Text;
				await sessionStateProvider.UpdateSessionStateByIdAsync(chatId, session, cancellationToken);
				
				if (session.RunCommandAfterWait)
				{
					var command = commands.Find(x => x.GetType().GetCustomAttribute<CommandAttribute>()?.Name == session.CommandName);
					if (command != null) await command.RunAsync(message, cancellationToken);
				}
			}
		}
	}
}