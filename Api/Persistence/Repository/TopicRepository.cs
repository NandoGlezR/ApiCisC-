using System.Diagnostics.CodeAnalysis;
using Domain.Entities;
using Domain.Repository;
using Domain.Views;
using MongoDB.Driver;
using Persistence.Context;

namespace Persistence.Repository;

[ExcludeFromCodeCoverage]
public class TopicRepository : RepositoryAsync<Topic, string>, ITopicRepository
{
    private readonly ApplicationContext _context;
    private readonly IIdeaRepository _ideaRepository;

    public TopicRepository(ApplicationContext context, IIdeaRepository ideaRepository) : base(context)
    {
        _context = context;
        _ideaRepository = ideaRepository;
    }

    public override async Task DeleteAsync(Topic entity)
    {
        foreach (var idea in await _ideaRepository.FindAllAsync(idea => idea.TopicId == entity.Id))
        {
            await _ideaRepository.DeleteAsync(idea);
        }

        await base.DeleteAsync(entity);
    }

    public async Task<PaginationView<TopicPagination>> GetPagedAsync(int page, int pageSize, string identifier = null)
    {
        var query = _context.Topics.AsQueryable()
            .Where(topic => identifier == null || topic.UserId == identifier)
            .Join(
                _context.Users.AsQueryable(), 
                topic => topic.UserId, 
                user => user.Id, 
                (topic, user) => new TopicPagination() 
                {
                    Identifier = topic.Id,
                    CreatedBy = user.UserName,
                    Title = topic.Title,
                });
        
        return await GetPagedOrderAsync(page, pageSize, query);
    }
}