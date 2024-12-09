using System.Diagnostics.CodeAnalysis;
using Domain.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

[ExcludeFromCodeCoverage]
public class User : BaseEntity
{

    [BsonElement("name")]
    public required string UserName { get; set; }

    [BsonElement("login")]
    public required string Email { get; set; }

    [BsonElement("password")]
    public required string Password { get; set; }
}