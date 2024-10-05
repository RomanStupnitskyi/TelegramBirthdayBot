using Birthday.Bot.Abstracts;
using Birthday.Bot.MongoProviders.Dto;
using MongoDB.Driver;

namespace Birthday.Bot.MongoProviders;

public class UserProvider(IMongoDatabase database) : BaseMongoProvider<UserDto>("Users", database)
{
	public async Task<UserDto> GetUserByIdAsync(long? userId)
	{
		return await Collection.Find(u => u.UserId == userId).FirstOrDefaultAsync();
	}
	
	public async Task<ReplaceOneResult> UpdateUserByIdAsync(UserDto user)
	{
		return await Collection.ReplaceOneAsync(u => u.UserId == user.UserId, user);
	}
	
	public async Task<DeleteResult> DeleteUserByIdAsync(long userId)
	{
		return await Collection.DeleteOneAsync(u => u.UserId == userId);
	}
}