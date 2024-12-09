using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repository;
using Domain.Views;
using Persistence.Context;

namespace Persistence.Repository;

public class IdeaRepository : RepositoryAsync<Idea, string>, IIdeaRepository
{
    private readonly ApplicationContext _context;
    public IdeaRepository(ApplicationContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PaginationView<IdeaPagination>> GetPagedAsync(int page, int pageSize, string identifier)
    {
        var query = _context.Ideas.Where(idea => idea.TopicId == identifier)
            .Join(_context.Users, idea => idea.UserId, user => user.Id,
                (idea, user) => new
                {
                    Idea = idea,
                    UserName = user.UserName,
                }).GroupJoin(_context.Votes,
                data => data.Idea.Id,
                vote => vote.IdeaId,(data, vote) => new IdeaPagination()
                {
                    Identifier = data.Idea.Id,
                    CreatedBy = data.UserName,
                    Description = data.Idea.Description,
                    Votes = vote.Count()
                });
        return await GetPagedOrderAsync(page, pageSize, query);
    }
}