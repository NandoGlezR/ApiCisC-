using System.Diagnostics.CodeAnalysis;
using Domain.Entities;
using Domain.Repository;
using Domain.Views;
using MongoDB.Driver;
using Persistence.Context;

namespace Persistence.Repository;

[ExcludeFromCodeCoverage]
public class IdeaRepository : RepositoryAsync<Idea, string>, IIdeaRepository
{
    private readonly ApplicationContext _context;
    public IdeaRepository(ApplicationContext context) : base(context)
    {
        _context = context;
    }

    public override Task DeleteAsync(Idea entity)
    {
        _context.Votes.DeleteMany(vote => vote.IdeaId == entity.Id);
        return base.DeleteAsync(entity);
    }

    public async Task<PaginationView<IdeaPagination>> GetPagedAsync(int page, int pageSize, string identifier)
    {
        
        var query = _context.Ideas.AsQueryable()
            .Where(idea => idea.TopicId == identifier)  
            .Join(
                _context.Users.AsQueryable(),          
                idea => idea.UserId,                   
                user => user.Id,                       
                (idea, user) => new                    
                {
                    Idea = idea,
                    UserName = user.UserName
                })
            .GroupJoin(
                _context.Votes.AsQueryable(),          
                data => data.Idea.Id,                  
                vote => vote.IdeaId,                   
                (data, votes) => new IdeaPagination() 
                {
                    Identifier = data.Idea.Id,
                    CreatedBy = data.UserName,
                    Description = data.Idea.Description,
                    Votes = votes.Count()              
                });
        
        return await GetPagedOrderAsync(page, pageSize, query);
    }
}