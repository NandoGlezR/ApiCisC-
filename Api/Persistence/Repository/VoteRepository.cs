using Domain.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repository;

public class VoteRepository : RepositoryAsync<Vote, string>, IVoteRepository
{
    private readonly DbSet<Vote> _entity;
    public VoteRepository(ApplicationContext context) : base(context)
    {
        _entity = context.Votes;
    }

    public async Task<long> CountVote(string ideaId)
    {
        return await _entity.LongCountAsync(vote => vote.IdeaId == ideaId);
    }
}