using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Core.Models;

public class UserOwnedEntity : BaseEntity
{
    [Column("id_user")] 
    public required string UserId { get; set; }
}