using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Core.Models;

public class BaseEntity
{
    [BsonId, BsonElement("id")]
    public required string Id { get; set; }
}