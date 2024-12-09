using Domain.Core.Models;
using Domain.Views;

namespace Application.Interfaces;

public interface IPaginationService<T> where T : BasePagination
{
    /// <summary>
    /// Retrieves a paginated view of the <typeparamref name="T"/> entity based on the specified page number, page size, and an optional identifier.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items each page should contain.</param>
    /// <param name="identifier">An optional identifier that can be used to filter or sort the results.</param>
    /// <returns>A task representing the asynchronous operation. The result contains an instance of <see cref="PaginationView{T}"/> with the paginated data.</returns>
    Task<PaginationView<T>> GetPageAsync(int pageNumber, int pageSize, string identifier = null);
}