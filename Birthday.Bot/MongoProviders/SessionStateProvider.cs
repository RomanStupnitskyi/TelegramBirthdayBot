using Birthday.Bot.Abstracts;
using Birthday.Bot.MongoProviders.Dto;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Birthday.Bot.MongoProviders;

public class SessionStateProvider(IMongoDatabase database) : BaseMongoProvider<SessionStateDto>("SessionStates", database)
{
	public async Task EnsureIndexesAsync(CancellationToken cancellationToken)
	{
		await CreateIndexIfNotExistsAsync(
			"AutoDelete",
			"CreatedAt",
			TimeSpan.FromMinutes(2),
			cancellationToken);
	}
	
	public async Task<SessionStateDto> GetSessionStateByIdAsync(long userId)
	{
		return await GetDocumentAsync(x => x.UserId == userId);
	}
	
	public async Task UpdateSessionStateByIdAsync(long userId, SessionStateDto updatedSessionState, CancellationToken cancellationToken)
	{
		Console.WriteLine($"Updating session state for user {userId} to {JsonConvert.SerializeObject(updatedSessionState)}");
		await UpdateUserAsync(
			x => x.UserId == userId,
			updatedSessionState,
			cancellationToken);
	}
	
	public async Task DeleteSessionStateByIdAsync(long userId, CancellationToken cancellationToken)
	{
		await DeleteUserAsync(x => x.UserId == userId, cancellationToken);
	}
}