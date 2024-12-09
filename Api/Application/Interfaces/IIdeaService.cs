using Application.Dto;
using Domain.Views;

namespace Application.Interfaces;

public interface IIdeaService : IPaginationService<IdeaPagination>
{
    /// <summary>
    /// Create a new idea in a topic 
    /// </summary>
    /// <param name="ideaDto">The DTO containing the information of the idea to create</param>
    /// <param name="userId">The Id of the user creating the idea</param>
    /// <param name="topicId">The Id of the topic where the idea will be created</param>
    /// <returns></returns>
    Task<IdeaDto> CreateIdeaAsync(IdeaDto ideaDto, string userId, string topicId);

    /// <summary>
    /// Updates an idea in an existing topic
    /// </summary>
    /// <param name="ideaDto">The Dto with the information of the idea to be updated</param>
    /// <param name="userId">The ID of the user who created the idea></param>
    /// <returns></returns>
    Task<IdeaDto> UpdateIdeaAsync(IdeaDto ideaDto, string userId);
    
    /// <summary>
    /// Deletes an idea in an existing topic
    /// </summary>
    /// <param name="ideaId">The id of the idea to be deleted</param>
    /// <param name="userId">The id of the user who created the idea</param>
    /// <returns></returns>
    Task<bool> DeleteIdeaAsync(string ideaId, string userId);
}