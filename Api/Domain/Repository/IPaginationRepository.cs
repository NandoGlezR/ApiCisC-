using Domain.Core.Models;
using Domain.Views;

namespace Domain.Repository;

public interface IPaginationRepository<T> where T : BasePagination
{
    /// <summary>
    /// Asynchronously retrieves a paginated list of items of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="page">The current page number to retrieve. Must be greater than 0.</param>
    /// <param name="pageSize">The number of items to include in each page. Must be greater than 0.</param>
    /// <param name="identifier">An optional identifier to filter the results. If provided, only items matching this identifier will be returned.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PaginationView{T}"/> object,
    /// which includes the paginated list of items and additional pagination information.</returns>
    Task<PaginationView<T>> GetPagedAsync(int page, int pageSize, string? identifier = null);
}