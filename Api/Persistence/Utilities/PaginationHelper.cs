using Domain.Core.Models;
using Domain.Views;
using MongoDB.Driver.Linq;

namespace Persistence.Utilities;

public abstract class PaginationHelper
{
    private const uint OutAllowedRange = 0;
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int SubtractPage = 1;

    /// <summary>
    /// Asynchronously retrieves a paginated list of items from the provided query.
    /// </summary>
    /// <typeparam name="T">The type of items being paginated, which must inherit from BasePagination.</typeparam>
    /// <param name="pageNumber">The current page number requested. If out of allowed range, it defaults to the first page.</param>
    /// <param name="pageSize">The number of items per page. If out of allowed range, it defaults to the predefined page size.</param>
    /// <param name="query">An IQueryable representing the data to be paginated.</param>
    /// <returns>A task that returns a PaginationView containing the paginated items and pagination metadata.</returns>
    protected async Task<PaginationView<T>> GetPagedOrderAsync<T>(int pageNumber, int pageSize, IQueryable<T> query)
        where T : BasePagination
    {
        if (pageNumber <= OutAllowedRange) pageNumber = DefaultPage;
        if (pageSize <= OutAllowedRange) pageSize = DefaultPageSize;
        var totalItems = await query.CountAsync();
        var views = await query.Skip((pageNumber - SubtractPage) * pageSize).Take(pageSize).ToListAsync();

        return new PaginationView<T>()
        {
            PageContent = views,
            FoundItems = totalItems,
            QuantityPerPage = pageSize,
            CurrentPage = pageNumber,
            PageAvailable = (int)Math.Ceiling((double)totalItems / pageSize),
        };
    }
}