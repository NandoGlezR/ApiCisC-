using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Domain.Core.Models;

namespace Domain.Entities;

[ExcludeFromCodeCoverage]
[Table("ideas")]
public class Idea : UserOwnedEntity
{
    [Key]
    [Column("id", TypeName = "varchar(36)")]
    public required string Id { get; set; } 

    [Column("description", TypeName = "varchar(255)")]
    public required string Description { get; set; }
    
    public User User { get; set; }
    
    [Column("id_topic")] 
    public required string TopicId { get; set; }
    public Topic Topic { get; set; }
    
    public ICollection<Vote> Votes { get; set; }
}