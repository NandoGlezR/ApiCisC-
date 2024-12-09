namespace Application.Interfaces;

public interface IVoteService
{
    /// <summary>
    /// Creates a vote for a specific idea.
    /// </summary>
    /// <param name="ideaId">The ID of the idea to which the vote is assigned.</param>
    /// <param name="userId">The ID of the user who creates the vote.</param>
    /// <returns>
    /// A <see cref="Task{bool}"/> that represents the asynchronous operation.
    /// The boolean value indicates whether the vote creation was successful or not.
    /// </returns>
    Task<bool> CreateVoteAsync(string ideaId, string userId);

    /// <summary>
    /// Deletes an existing vote for a specific idea.
    /// </summary>
    /// <param name="ideaId">The ID of the idea from which the vote will be removed.</param>
    /// <param name="userId">The ID of the user who deleted the vote.</param>
    /// <returns>
    /// A <see cref="Task{bool}"/> that represents the asynchronous operation.
    /// The boolean value indicates whether the vote deletion was successful or not.
    /// </returns>
    Task<bool> DeleteVoteAsync(string ideaId, string userId);

    /// <summary>
    /// Retrieves the total number of votes for a specific idea.
    /// </summary>
    /// <param name="ideaId">The ID of the idea for which the votes are counted.</param>
    /// <returns>
    /// A <see cref="Task{long}"/> that represents the asynchronous operation.
    /// The long value represents the number of votes that the idea has.
    /// </returns>
    Task<long> GetNumberVotesAsync(string ideaId);
}