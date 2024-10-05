using Birthday.Bot.Abstracts;
using Birthday.Bot.Attributes;
using Birthday.Bot.MongoProviders;
using Birthday.Bot.MongoProviders.Dto;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Birthday.Bot.Commands;

[Command("birthday", "Set your birthday")]
public class BirthdayCommand(ITelegramBotClient telegramBotClient, UserProvider userProvider, SessionStateProvider sessionStateProvider) : ICommand
{
	public async Task RunAsync(Message message, CancellationToken cancellationToken)
	{
		var sessionState = await sessionStateProvider.GetSessionStateByIdAsync(message.Chat.Id);
		var user = await userProvider.GetUserByIdAsync(message.Chat.Id);

		if (user != null)
		{
			await UpdateUserBirthday(message, user, sessionState, cancellationToken);
			return;
		}
		
		if (sessionState == null)
		{
			sessionState = new SessionStateDto(
				message.Chat.Id,
				"",
				true,
				"birthday",
				DateTime.UtcNow);
			await sessionStateProvider.AddDocumentAsync(sessionState, cancellationToken);
			
			await telegramBotClient.SendTextMessageAsync(
				message.Chat.Id,
				"\ud83c\udf88 Напиши свій день народження у форматі ДД.ММ.РРРР (наприклад, 26.01.2006)",
				cancellationToken: cancellationToken);
			return;
		}
		if (sessionState.State == "" || !IsDateValid(sessionState.State ?? ""))
		{
			await telegramBotClient.SendTextMessageAsync(
				message.Chat.Id,
				"\u274c Неправильний формат дати. Напиши свій день народження у форматі ДД.ММ.РРРР (наприклад, 26.01.2006)",
				cancellationToken: cancellationToken);
		}
		else
		{
			var birthday = ParseDate(sessionState.State ?? "");
			await userProvider.AddDocumentAsync(new UserDto(message.Chat.Id, birthday), cancellationToken);
			
			await telegramBotClient.SendTextMessageAsync(
				message.Chat.Id,
				"\u2705 День народження успішно встановлено!",
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
			await telegramBotClient.SendTextMessageAsync(
				message.Chat.Id,
				"\ud83d\udd04 Щоб змінити свою дату народження напиши її в текстовому форматі ДД.ММ.РРРР (наприклад, 26.01.2006)",
				cancellationToken: cancellationToken);
			return;
		}
		
		if (!IsDateValid(sessionState.State))
		{
			await telegramBotClient.SendTextMessageAsync(
				message.Chat.Id,
				"\u274c Неправильний формат дати. Напиши свій день народження у форматі ДД.ММ.РРРР (наприклад, 26.01.2006)",
				cancellationToken: cancellationToken);
			return;
		}

		user.Birthday = ParseDate(sessionState.State);
		await userProvider.UpdateUserAsync(x => x.UserId == message.Chat.Id, user, cancellationToken);
		await sessionStateProvider.DeleteSessionStateByIdAsync(message.Chat.Id, cancellationToken);
		
		await telegramBotClient.SendTextMessageAsync(
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