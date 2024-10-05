using System.Linq.Expressions;
using Birthday.Bot.MongoProviders.Dto;
using MongoDB.Driver;

namespace Birthday.Bot.Abstracts;

public class BaseMongoProvider<T>(string collectionName, IMongoDatabase database)
{
	protected readonly IMongoCollection<T> Collection = database.GetCollection<T>(collectionName);
	
	public async Task CreateIndexIfNotExistsAsync(string indexName, string fieldName, TimeSpan expireAfterDelay, CancellationToken cancellationToken)
	{
		if ((await Collection.Indexes.ListAsync(cancellationToken)).ToList(cancellationToken: cancellationToken).Any(index => index["name"] == indexName))
			return;
		
		var indexKeys = Builders<T>.IndexKeys.Ascending(fieldName);
		var indexOptions = new CreateIndexOptions
		{
			Name = indexName,
			ExpireAfter = expireAfterDelay
		};

		var indexModel = new CreateIndexModel<T>(indexKeys, indexOptions);
		await Collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
	}
	
	public async Task<T> GetDocumentAsync(Expression<Func<T, bool>> where)
	{
		return await Collection.Find(where).FirstOrDefaultAsync();
	}
	
	public async Task<IEnumerable<T>> GetDocumentsAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken)
	{
		return await Collection.Find(where).ToListAsync(cancellationToken: cancellationToken);
	}
	
	public async Task AddDocumentAsync(T document, CancellationToken cancellationToken)
	{
		await Collection.InsertOneAsync(document, cancellationToken: cancellationToken);
	}
	
	public async Task<ReplaceOneResult> UpdateUserAsync(Expression<Func<T, bool>> where, T updatedUser, CancellationToken cancellationToken)
	{
		return await Collection.ReplaceOneAsync(where, updatedUser, cancellationToken: cancellationToken);
	}
	
	public async Task<DeleteResult> DeleteUserAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken)
	{
		return await Collection.DeleteOneAsync(where, cancellationToken);
	}
}