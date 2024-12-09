using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Domain.Core.Models;

namespace Domain.Entities;

[ExcludeFromCodeCoverage]
[Table("topics")]
public class Topic : UserOwnedEntity
{
    [Key]
    [Column("id", TypeName = "varchar(36)")]
    public required string Id { get; set; }

    [Column("title", TypeName = "varchar(255)")]
    public required string Title { get; set; }
    
    public User User { get; set; }
    public ICollection<Idea> Ideas { get; set; }
}