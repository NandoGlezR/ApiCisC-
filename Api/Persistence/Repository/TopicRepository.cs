using Domain.Entities;
using Domain.Repository;
using Domain.Views;
using Persistence.Context;

namespace Persistence.Repository;

public class TopicRepository : RepositoryAsync<Topic, string>, ITopicRepository
{
    private readonly ApplicationContext _context;

    public TopicRepository(ApplicationContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<PaginationView<TopicPagination>> GetPagedAsync(int page, int pageSize, string identifier = null)
    {
        var query = _context.Topics.Where(topic => identifier == null || topic.UserId == identifier)
            .Join(_context.Users, topic => topic.UserId, user => user.Id,
                (topic, user) => new TopicPagination()
                {
                    Identifier = topic.Id,
                    CreatedBy = user.UserName,
                    Title = topic.Title,
                });
        return await GetPagedOrderAsync(page, pageSize, query);
    }
}