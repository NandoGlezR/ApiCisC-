using System.Diagnostics.CodeAnalysis;
using Domain.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

[ExcludeFromCodeCoverage]
public class Idea : UserOwnedEntity
{

    [BsonElement("description")]
    public required string Description { get; set; }
    
    [BsonElement("topic_id")] 
    public required string TopicId { get; set; }
    
}