using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Domain.Core.Models;

namespace Domain.Entities;

[ExcludeFromCodeCoverage]
[Table("users")]
public class User : BaseEntity
{
    [Key]
    [MaxLength(36)]
    [Column("id", TypeName = "varchar(36)")]
    public required string Id { get; set; }

    [Column("name", TypeName = "varchar(255)")]
    public required string UserName { get; set; }

    [Column("login", TypeName = "varchar(255)")]
    public required string Email { get; set; }

    [Column("password", TypeName = "varchar(255)")]
    public required string Password { get; set; }

    public ICollection<Topic> Topics { get; set; }
    public ICollection<Idea> Ideas { get; set; }
    public ICollection<Vote> Votes {get; set;}
}