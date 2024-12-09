using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Core.Models;

public class UserOwnedEntity : BaseEntity
{
    [BsonElement("user_id")] 
    public required string UserId { get; set; }
}