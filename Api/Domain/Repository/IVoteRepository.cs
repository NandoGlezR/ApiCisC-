using Domain.Entities;

namespace Domain.Repository;

public interface IVoteRepository : IRepositoryAsync<Vote, string>
{
    Task<long> CountVote(string ideaId);
}