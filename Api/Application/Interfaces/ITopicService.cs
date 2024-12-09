using Application.Dto;
using Domain.Views;

namespace Application.Interfaces;

public interface ITopicService : IPaginationService<TopicPagination>
{
    /// <summary>
    /// Creates a new topic for the specified user.
    /// </summary>
    /// <param name="topicDto">The DTO containing the information of the topic to create.</param>
    /// <param name="userId">The ID of the user creating the topic.</param>
    /// <returns>The DTO of the created topic.</returns>
    Task<TopicDto> CreateTopicAsync(TopicDto topicDto, string userId);

    /// <summary>
    /// Updates an existing topic that belongs to the specified user.
    /// </summary>
    /// <param name="topicDto">The DTO with the updated topic information.</param>
    /// <param name="userId">The ID of the user who owns the topic.</param>
    /// <returns>The DTO of the updated topic.</returns>
    Task<TopicDto> UpdateTopicAsync(TopicDto topicDto, string userId);

    /// <summary>
    /// Deletes a topic that belongs to the specified user.
    /// </summary>
    /// <param name="topicId">The ID of the topic to delete.</param>
    /// <param name="userId">The ID of the user who owns the topic.</param>
    /// <returns>A boolean value indicating whether the delete operation was successful.</returns>
    Task<bool> DeleteTopicAsync(string topicId, string userId);
    
    /// <summary>
    /// Retrieves a specific topic by its ID.
    /// This method returns the topic details associated with the given topic ID.
    /// </summary>
    /// <param name="topicId">The ID of the topic to retrieve.</param>
    /// <returns>Contains the Dto corresponding to the specified topic ID.</returns>
    Task<TopicDto> GetTopicByIdAsync(string topicId);
}