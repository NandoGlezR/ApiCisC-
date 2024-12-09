using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Domain.Core.Models;
using Domain.Repository;
using MongoDB.Driver;
using Persistence.Context;
using Persistence.Utilities;

namespace Persistence.Repository;

[ExcludeFromCodeCoverage]
public class RepositoryAsync<T, TKey> : PaginationHelper, IRepositoryAsync<T, TKey> where T : BaseEntity where TKey : IEquatable<TKey>
{
    private readonly ApplicationContext _context;
    private readonly IMongoCollection<T> _entities;
    public RepositoryAsync(ApplicationContext context)
    {
        _context = context;
        _entities = _context.Set<T>();
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _entities.Find(_ => true).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _entities.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        var filter = Builders<T>.Filter.Eq("_id", entity.Id);
        await _entities.ReplaceOneAsync(filter, entity);
    }

    public virtual async Task DeleteAsync(T entity)
    {
        var filter = Builders<T>.Filter.Eq("_id", entity.Id);
        await _entities.DeleteOneAsync(filter);
    }

    public async Task<T> FindByIdAsync(TKey id)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);
        return await _entities.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entities.Find(predicate).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entities.Find(predicate).ToListAsync();
    }
    
}