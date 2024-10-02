namespace Birthday.Bot.MongoProviders.Dto;

public record SessionStateDto(long userId, string waitingFor, bool runCommandAfterWait, string commandName, DateTime createdAt)
{
}