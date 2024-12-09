using System.Linq.Expressions;
using Domain.Core.Models;

namespace Domain.Repository;

public interface IRepositoryAsync<T, in TKey> where T : BaseEntity where TKey : IEquatable<TKey>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T> FindByIdAsync(params object[] keyValues);
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
}