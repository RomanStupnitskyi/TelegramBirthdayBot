using Birthday.Bot.MongoProviders;
using MongoDB.Driver;

namespace Birthday.Bot.Services;

public class MongoDatabaseService : MongoClient
{
	public readonly UserProvider UserProvider;
	public readonly SessionStateProvider SessionStateProvider;
	
	public MongoDatabaseService(string connectionString) : base(connectionString)
	{
		var database = GetDatabase("Birthday");
		
		UserProvider = new UserProvider(database);
		SessionStateProvider = new SessionStateProvider(database);
	}
}