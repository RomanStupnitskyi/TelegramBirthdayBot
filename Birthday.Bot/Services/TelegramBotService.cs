using Birthday.Bot.Abstracts;
using Birthday.Bot.Commands;
using Birthday.Bot.Listeners;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Reflection;
using Birthday.Bot.Attributes;

namespace Birthday.Bot.Services;

public interface ITelegramBotService
{
	public List<IListener> OnMessage { get; init; }
	public List<IListener> OnInlineQuery { get; init; }
	public List<IListener> OnCallbackQuery { get; init; }
	public List<ICommand> Commands { get; init; }
	Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
	Task UpdateBotCommandsAsync(CancellationToken cancellationToken);
}

public class TelegramBotService : ITelegramBotService
{
	private readonly ITelegramBotClient _telegramBotClient;
	public List<IListener> OnMessage { get; init; }
	public List<IListener> OnInlineQuery { get; init; }
	public List<IListener> OnCallbackQuery { get; init; }
	public List<ICommand> Commands { get; init; }
	
	public TelegramBotService(ITelegramBotClient telegramBotClient, List<IListener>? onInlineQuery = null, List<IListener>? onCallbackQuery = null)
	{
		_telegramBotClient = telegramBotClient;
		OnInlineQuery = onInlineQuery ?? [];
		OnCallbackQuery = onCallbackQuery ?? [];

		Commands = [ new StartCommand(telegramBotClient) ];
		OnMessage = [ new MessageListener(Commands) ];
	}

	public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
	{
		var listeners = update.Type switch
		{
			UpdateType.Message => OnMessage,
			UpdateType.InlineQuery => OnInlineQuery,
			UpdateType.CallbackQuery => OnCallbackQuery,
			_ => null
		};
		
		if (listeners == null) return;
		foreach (var listener in listeners)
		{
			await listener.RunAsync(update, cancellationToken);
		}
	}
	
	public async Task UpdateBotCommandsAsync(CancellationToken cancellationToken)
	{
		List<BotCommand> commands = [];
		commands.AddRange(
			from attribute
			in Commands
				.Select(command => command.GetType().GetCustomAttribute<CommandAttribute>())
				.OfType<CommandAttribute>()
			where !attribute.IsHidden
			select new BotCommand { Command = attribute.Name, Description = attribute.Description }
		);

		await _telegramBotClient.SetMyCommandsAsync(commands, cancellationToken: cancellationToken);
	}
}