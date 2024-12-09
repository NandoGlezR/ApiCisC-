using System.Diagnostics.CodeAnalysis;
using Domain.Entities;
using Domain.Repository;
using MongoDB.Driver;
using Persistence.Context;

namespace Persistence.Repository;

[ExcludeFromCodeCoverage]
public class VoteRepository : RepositoryAsync<Vote, string>, IVoteRepository
{
    private readonly IMongoCollection<Vote> _entity;
    public VoteRepository(ApplicationContext context) : base(context)
    {
        _entity = context.Votes;
    }

    public async Task<long> CountVote(string ideaId)
    {
        return await _entity.CountDocumentsAsync(vote => vote.IdeaId == ideaId);
    }
}