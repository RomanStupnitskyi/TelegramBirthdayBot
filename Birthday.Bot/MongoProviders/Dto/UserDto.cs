using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Birthday.Bot.MongoProviders.Dto;

public record UserDto(long UserId, DateTime Birthday, string? Id = null)
{
	public DateTime Birthday { get; set; }
	[BsonElement("_id")]
	[JsonProperty("_id")]
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string Id { get; set; }
}