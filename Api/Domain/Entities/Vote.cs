using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Domain.Core.Models;

namespace Domain.Entities;

[ExcludeFromCodeCoverage]
[Table("votes")]
public class Vote : UserOwnedEntity
{
    [Column("id_idea", TypeName = "varchar(36)")]
    public required string IdeaId {get; set;}
    
    public User User {get; set;}
    public Idea Idea {get; set;}
}