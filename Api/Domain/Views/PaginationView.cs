using System.ComponentModel.DataAnnotations;
using Domain.Core.Models;

namespace Domain.Views;

public class PaginationView<T>
{
    public IEnumerable<T> PageContent { get; set; } = Enumerable.Empty<T>();
    public int FoundItems { get; set; }
    public int QuantityPerPage { get; set; }
    public int CurrentPage { get; set; }
    public int PageAvailable { get; set; }
}