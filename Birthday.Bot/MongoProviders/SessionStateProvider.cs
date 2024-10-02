using Birthday.Bot.Abstracts;
using Birthday.Bot.MongoProviders.Dto;
using MongoDB.Driver;

namespace Birthday.Bot.MongoProviders;

public class SessionStateProvider(IMongoDatabase database) : BaseMongoProvider<SessionStateDto>("SessionStates", database)
{
	public async Task EnsureIndexesAsync()
	{
		await CreateIndexIfNotExistsAsync("AutoDelete", "createdAt", TimeSpan.FromMinutes(2));
	}
	
	public async Task<SessionStateDto> GetSessionStateByIdAsync(long userId)
	{
		return await GetDocumentAsync(x => x.userId == userId);
	}
	
	public async Task UpdateSessionByIdStateAsync(long userId, SessionStateDto updatedSessionState)
	{
		await UpdateUserAsync(x => x.userId == userId, updatedSessionState);
	}
	
	public async Task DeleteSessionStateByIdAsync(long userId)
	{
		await DeleteUserAsync(x => x.userId == userId);
	}
}