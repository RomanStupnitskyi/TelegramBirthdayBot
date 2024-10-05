using Birthday.Bot.Abstracts;
using Birthday.Bot.Attributes;
using Telegram.Bot.Types;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Birthday.Bot.Handlers;

[Handler("Message")]
public class CommandsHandler(ITelegramBotClient telegramBotClient, List<ICommand> commands) : IHandler
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
		if (message != null && command != null && message.From != null)
		{
			var commandAttribute = command.GetType().GetCustomAttribute<CommandAttribute>();
			var chatMember = await telegramBotClient.GetChatMemberAsync(message.Chat.Id, message!.From.Id, cancellationToken);
			var chatType = message.Chat.Type;

			if (commandAttribute is null)
				return;
			
			if (CheckUserHasPermissions(message, chatMember, commandAttribute))
			{
				switch (chatType)
				{
					case ChatType.Private:
						await HandleAccessAsync(message, command, commandAttribute.AllowPrivate, cancellationToken);
						break;
					case ChatType.Group or ChatType.Supergroup:
						await HandleAccessAsync(message, command, commandAttribute.AllowGroup, cancellationToken);
						break;
					case ChatType.Channel:
						await HandleAccessAsync(message, command, commandAttribute.AllowChannel, cancellationToken);
						break;
					default:
						await command.NoAccessAsync(message, cancellationToken);
						break;
				}
			}
			else
			{
				await command.NoPermissionAsync(message, cancellationToken);
			}
		}
	}
	
	private static bool CheckUserHasPermissions(
		Message message,
		ChatMember chatMember,
		CommandAttribute commandAttribute)
	{
		if (commandAttribute.IsAdminOnly && message.Chat.Type is ChatType.Group or ChatType.Supergroup)
			return chatMember.Status is ChatMemberStatus.Administrator or ChatMemberStatus.Creator || chatMember.User.Id == 1234567890;
		if (commandAttribute.IsOwnerOnly)
			return chatMember.User.Id == 1234567890;
		
		return true;
	}
	
	private static async Task HandleAccessAsync(Message message, ICommand command, bool hasAccess, CancellationToken cancellationToken)
	{
		if (hasAccess)
		{
			await command.RunAsync(message, cancellationToken);
		}
		else
		{
			await command.NoAccessAsync(message, cancellationToken);
		}
	}
}