using System.Linq.Expressions;
using Domain.Core.Models;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Utilities;

namespace Persistence.Repository;

public class RepositoryAsync<T, TKey> : PaginationHelper, IRepositoryAsync<T, TKey> where T : BaseEntity where TKey : IEquatable<TKey>
{
    private readonly ApplicationContext _context;
    private readonly DbSet<T> _entities;
    public RepositoryAsync(ApplicationContext context)
    {
        _context = context;
        _entities = _context.Set<T>();
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _entities.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _entities.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _entities.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _entities.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<T> FindByIdAsync(params object[] keyValues)
    {
        return await _entities.FindAsync(keyValues);
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entities.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entities.Where(predicate).ToListAsync();
    }
    
}