using System.ComponentModel.DataAnnotations;
using Domain.Core.Models;

namespace Domain.Views;

public class IdeaPagination : BasePagination
{
    public required string CreatedBy { get; set; }
    public required string Description { get; set; }
    public required long Votes { get; set; }
}