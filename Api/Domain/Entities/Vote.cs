using System.Diagnostics.CodeAnalysis;
using Domain.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

[ExcludeFromCodeCoverage]
public class Vote : UserOwnedEntity
{
    [BsonElement("idea_id")]
    public required string IdeaId {get; set;}
}