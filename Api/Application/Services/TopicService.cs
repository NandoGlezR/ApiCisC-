using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Repository;
using Domain.Views;
using FluentValidation;

namespace Application.Services;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<TopicDto> _topicValidator;
    private readonly IEntityValidator _entityValidator;

    public TopicService(ITopicRepository repository, IMapper mapper,
        IValidator<TopicDto> topicValidator, IEntityValidator entityValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _topicValidator = topicValidator;
        _entityValidator = entityValidator;
    }

    public async Task<TopicDto> CreateTopicAsync(TopicDto topicDto, string userId)
    {
        ValidateInputs(topicDto, userId);
        topicDto.Id = Guid.NewGuid().ToString();
        var topic = _mapper.Map<Topic>(topicDto);
        topic.UserId = userId;
        await _repository.AddAsync(topic);
        return topicDto;
    }

    public async Task<TopicDto> UpdateTopicAsync(TopicDto topicDto, string userId)
    {
        ValidateInputs(topicDto, userId, isUpdate: true);
        var topic = await _repository.FirstOrDefaultAsync(topic => topic.Id == topicDto.Id && topic.UserId == userId);
        _entityValidator.ValidateEntityIsNotNull(topic);
        topic.Title = topicDto.Title;
        await _repository.UpdateAsync(topic);
        return topicDto;
    }

    public async Task<bool> DeleteTopicAsync(string topicId, string userId)
    {
        _entityValidator.ValidateStringField(topicId, isIdentifier: true);
        _entityValidator.ValidateStringField(userId, isIdentifier: true);
        var topic = await _repository.FirstOrDefaultAsync(topic => topic.Id == topicId && topic.UserId == userId);
        _entityValidator.ValidateEntityIsNotNull(topic);
        await _repository.DeleteAsync(topic);
        return true;
    }
    
    public async Task<TopicDto> GetTopicByIdAsync(string topicId)
    {
        _entityValidator.ValidateStringField(topicId, isIdentifier: true);
        var topic = await _repository.FindByIdAsync(topicId);
        _entityValidator.ValidateEntityIsNotNull(topic);
        return _mapper.Map<TopicDto>(topic);
    }
    
    public async Task<PaginationView<TopicPagination>> GetPageAsync(int pageNumber, int pageSize, string identifier = null)
    {
        return await _repository.GetPagedAsync(pageNumber, pageSize, identifier);
    }
    
    /// <summary>
    /// Validates the input data for creating or updating a topic.
    /// This method is unique for this service, since the validation is only unique for this topic
    /// </summary>
    /// <param name="topicDto">The DTO containing the topic data to validate.</param>
    /// <param name="userId">The ID of the user associated with the topic.</param>
    /// <param name="isUpdate">Indicates whether the validation is for an update operation. Default is false.</param>
    /// <exception cref="ValidationException">Thrown when the validation of the topic fails.</exception>
    /// <exception cref="NullReferenceException">Thrown when the userId is null or empty.</exception>
    private void ValidateInputs(TopicDto topicDto, string userId, bool isUpdate = false)
    {
        var topicValidation = isUpdate
            ? _topicValidator.Validate(topicDto, options => options.IncludeRuleSets("update"))
            : _topicValidator.Validate(topicDto);

        if (!topicValidation.IsValid)
        {
            throw new ValidationException(topicValidation.Errors);
        }

        _entityValidator.ValidateStringField(userId, isIdentifier: true);
    }
}