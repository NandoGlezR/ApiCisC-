using System.Diagnostics.CodeAnalysis;
using Domain.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

[ExcludeFromCodeCoverage]
public class Topic : UserOwnedEntity
{

    [BsonElement("title")]
    public required string Title { get; set; }
}