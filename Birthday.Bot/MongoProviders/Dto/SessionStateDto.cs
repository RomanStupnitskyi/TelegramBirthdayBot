using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Birthday.Bot.MongoProviders.Dto;

public record SessionStateDto(long UserId, string? State, bool RunCommandAfterWait, string CommandName, DateTime CreatedAt)
{
	public string? State { get; set; } = State;
	public ObjectId? _id { get; init; } = ObjectId.GenerateNewId();
}