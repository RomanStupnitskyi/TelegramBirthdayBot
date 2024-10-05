using Birthday.Bot.Abstracts;
using Birthday.Bot.Commands;
using Birthday.Bot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Reflection;
using Birthday.Bot.Attributes;

namespace Birthday.Bot.Services;

public interface ITelegramBotService
{
	public List<IHandler> OnMessage { get; init; }
	public List<IHandler> OnInlineQuery { get; init; }
	public List<IHandler> OnCallbackQuery { get; init; }
	public List<ICommand> Commands { get; init; }
	Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
	Task UpdateBotCommandsAsync(CancellationToken cancellationToken);
}

public class TelegramBotService : ITelegramBotService
{
	private readonly ITelegramBotClient _telegramBotClient;
	public List<IHandler> OnMessage { get; init; }
	public List<IHandler> OnInlineQuery { get; init; }
	public List<IHandler> OnCallbackQuery { get; init; }
	public List<ICommand> Commands { get; init; }
	
	public TelegramBotService(
		ITelegramBotClient telegramBotClient,
		MongoDatabaseService mongoDatabaseService,
		List<IHandler>? onInlineQuery = null,
		List<IHandler>? onCallbackQuery = null)
	{
		_telegramBotClient = telegramBotClient;
		OnInlineQuery = onInlineQuery ?? [];
		OnCallbackQuery = onCallbackQuery ?? [];

		Commands = [
			new StartCommand(telegramBotClient),
			new BirthdayCommand(telegramBotClient, mongoDatabaseService.UserProvider, mongoDatabaseService.SessionStateProvider),
			new ShowBirthdayCommand(telegramBotClient, mongoDatabaseService.UserProvider)
		];
		OnMessage = [
			new SessionHandler(Commands, mongoDatabaseService.SessionStateProvider),
			new CommandsHandler(telegramBotClient, Commands)
		];
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