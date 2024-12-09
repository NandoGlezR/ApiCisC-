using Domain.Core.Models;

namespace Domain.Views;

public class TopicPagination : BasePagination
{
    public required string CreatedBy { get; set; }
    public required string Title { get; set; }
}