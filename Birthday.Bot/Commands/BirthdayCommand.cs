using Birthday.Bot.Abstracts;
using Birthday.Bot.Attributes;
using Birthday.Bot.MongoProviders;
using Birthday.Bot.MongoProviders.Dto;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Birthday.Bot.Commands;

[Command("birthday", "Set your birthday", isOwnerOnly: true)]
public class BirthdayCommand(
	ITelegramBotClient telegramBotClient,
	UserProvider userProvider,
	SessionStateProvider sessionStateProvider) : BaseCommand(telegramBotClient)
{
	private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;

	public override async Task RunAsync(Message message, CancellationToken cancellationToken)
	{
		if (message.From != null)
		{
			var sessionState = await sessionStateProvider.GetSessionStateByIdAsync(message.From.Id);
			var user = await userProvider.GetUserByIdAsync(message.From.Id);

			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (user != null)
			{
				await UpdateUserBirthday(message, user, sessionState, cancellationToken);
				return;
			}
		
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (sessionState == null)
			{
				sessionState = new SessionStateDto(
					message.Chat.Id,
					"",
					true,
					"birthday",
					DateTime.UtcNow);
				await sessionStateProvider.AddDocumentAsync(sessionState, cancellationToken);
			
				await _telegramBotClient.SendTextMessageAsync(
					message.Chat.Id,
					"\ud83c\udf88 Напиши свій день народження у форматі ДД.ММ.РРРР (наприклад, 26.01.2006)",
					cancellationToken: cancellationToken);
				return;
			}
			if (sessionState.State == "" || !IsDateValid(sessionState.State ?? ""))
			{
				await _telegramBotClient.SendTextMessageAsync(
					message.Chat.Id,
					"\u274c Неправильний формат дати. Напиши свій день народження у форматі ДД.ММ.РРРР (наприклад, 26.01.2006)",
					cancellationToken: cancellationToken);
			}
			else
			{
				var birthday = ParseDate(sessionState.State ?? "");
				await userProvider.AddDocumentAsync(new UserDto(message.Chat.Id, birthday), cancellationToken);
			
				await _telegramBotClient.SendTextMessageAsync(
					message.Chat.Id,
					"\u2705 День народження успішно встановлено!",
					cancellationToken: cancellationToken);
			}
		}
	}
	
	public override async Task NoAccessAsync(Message message, CancellationToken cancellationToken)
	{
		if (message.From != null)
		{
			var user = await userProvider.GetUserByIdAsync(message.From.Id);
		
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (user == null)
			{
				await _telegramBotClient.SendTextMessageAsync(
					message.Chat.Id,
					"\ud83e\udd37\u200d\u2642\ufe0f Ти ще не вказав свій день народження",
					cancellationToken: cancellationToken);
				return;
			}
		
			await _telegramBotClient.SendTextMessageAsync(
				message.Chat.Id,
				$"\ud83c\udf89 Твоє день народження: <b>{user.Birthday:dd.MM.yyyy}</b>",
				parseMode: ParseMode.Html,
				cancellationToken: cancellationToken);
		}
	}
	
	private async Task UpdateUserBirthday(Message message, UserDto user, SessionStateDto? sessionState, CancellationToken cancellationToken)
	{
		if (sessionState == null || sessionState.State == "")
		{
			if (sessionState == null)
			{
				sessionState = new SessionStateDto(
					message.Chat.Id,
					"",
					true,
					"birthday",
					DateTime.UtcNow);
				await sessionStateProvider.AddDocumentAsync(sessionState, cancellationToken);
			}
			await _telegramBotClient.SendTextMessageAsync(
				message.Chat.Id,
				"\ud83d\udd04 Щоб змінити свою дату народження напиши її в текстовому форматі ДД.ММ.РРРР (наприклад, 26.01.2006)",
				cancellationToken: cancellationToken);
			return;
		}
		
		if (!IsDateValid(sessionState.State ?? ""))
		{
			await _telegramBotClient.SendTextMessageAsync(
				message.Chat.Id,
				"\u274c Неправильний формат дати. Напиши свій день народження у форматі ДД.ММ.РРРР (наприклад, 26.01.2006)",
				cancellationToken: cancellationToken);
			return;
		}

		user.Birthday = ParseDate(sessionState.State ?? "");
		await userProvider.UpdateUserAsync(x => x.UserId == message.Chat.Id, user, cancellationToken);
		await sessionStateProvider.DeleteSessionStateByIdAsync(message.Chat.Id, cancellationToken);
		
		await _telegramBotClient.SendTextMessageAsync(
			message.Chat.Id,
			"\u2705 Твій день народження успішно оновлено!",
			cancellationToken: cancellationToken);
	}
	
	private static DateTime ParseDate(string date)
	{
		var dateList = date.Split('.');
		return DateTime.Parse(string.Join('/', dateList)).ToLocalTime();
	}
	
	private static bool IsDateValid(string date)
	{
		var dateList = date.Split('.');
		return dateList.Length >= 3 && DateTime.TryParse(string.Join('/', dateList.Reverse()), out _);
	}
}