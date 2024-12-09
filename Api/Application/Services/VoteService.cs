using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repository;

namespace Application.Services;

public class VoteService : IVoteService
{
    private readonly IVoteRepository _voteRepository;
    private readonly IEntityValidator _entityValidator;
    public VoteService(IVoteRepository voteRepository, IEntityValidator entityValidator)
    {
        _voteRepository = voteRepository;
        _entityValidator = entityValidator;
    }

    public async Task<bool> CreateVoteAsync(string ideaId, string userId)
    {
        _entityValidator.ValidateStringField(ideaId, isIdentifier: true);
        var vote =  await _voteRepository.FirstOrDefaultAsync(vote => vote.IdeaId == ideaId && vote.UserId == userId);

        if (vote != null) return false;

        var newVote = new Vote
        {
            IdeaId = ideaId,
            UserId = userId
        };

        await _voteRepository.AddAsync(newVote);
        return true;
    }

    public async Task<bool> DeleteVoteAsync(string ideaId, string userId)
    {
        _entityValidator.ValidateStringField(ideaId, isIdentifier: true);
        var vote = await _voteRepository.FirstOrDefaultAsync(vote => vote.IdeaId == ideaId && vote.UserId == userId);
        if(vote == null ) return false;
        await _voteRepository.DeleteAsync(vote);
        return true;
    }

    public async Task<long> GetNumberVotesAsync(string ideaId)
    {
        _entityValidator.ValidateStringField(ideaId, isIdentifier: true);
        return await _voteRepository.CountVote(ideaId);
    }
}