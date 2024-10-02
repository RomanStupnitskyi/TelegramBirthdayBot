using System.Linq.Expressions;
using Birthday.Bot.Abstracts;
using Birthday.Bot.MongoProviders.Dto;
using MongoDB.Driver;

namespace Birthday.Bot.MongoProviders;

public class UserProvider(IMongoDatabase database) : BaseMongoProvider<UserDto>("Users", database)
{
	async Task<UserDto> GetUserByIdAsync(long userId)
	{
		return await Collection.Find(u => u.UserId == userId).FirstOrDefaultAsync();
	}
	
	async Task<ReplaceOneResult> UpdateUserByIdAsync(UserDto user)
	{
		return await Collection.ReplaceOneAsync(u => u.UserId == user.UserId, user);
	}
	
	async Task<DeleteResult> DeleteUserByIdAsync(long userId)
	{
		return await Collection.DeleteOneAsync(u => u.UserId == userId);
	}
}