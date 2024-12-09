using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Repository;
using Domain.Views;
using FluentValidation;

namespace Application.Services;

public class IdeaService : IIdeaService
{
    private readonly IIdeaRepository _repositoryIdea;
    private readonly IMapper _mapper;
    private readonly IValidator<IdeaDto> _ideaValidator;
    private readonly IEntityValidator _entityValidator;
    private readonly ITopicService _topicService;
    
    public IdeaService(IIdeaRepository repositoryIdea, IMapper mapper,
        IValidator<IdeaDto> ideaValidator, IEntityValidator entityValidator)
    {
        _repositoryIdea = repositoryIdea;
        _mapper = mapper;
        _ideaValidator = ideaValidator;
        _entityValidator = entityValidator;
    }


    public async Task<IdeaDto> CreateIdeaAsync(IdeaDto ideaDto, string userId, string topicId)
    { 
        _entityValidator.ValidateStringField(topicId);
        ValidateInputs(ideaDto, userId);
        var idea = _mapper.Map<Idea>(ideaDto);
        idea.UserId = userId;
        idea.TopicId = topicId;
        idea.Id = Guid.NewGuid().ToString(); 
        ideaDto.Id = idea.Id;  
        await _repositoryIdea.AddAsync(idea);
        return ideaDto;
    }
    
    public async Task<IdeaDto> UpdateIdeaAsync(IdeaDto ideaDto, string userId)
    {
        ValidateInputs(ideaDto, userId, isUpdate: true);
        var idea = await _repositoryIdea.FirstOrDefaultAsync(idea => idea.Id == ideaDto.Id && idea.UserId == userId);
        _entityValidator.ValidateEntityIsNotNull(idea);
        idea.Description = ideaDto.Description;
        await _repositoryIdea.UpdateAsync(idea);
        return ideaDto;
    }

    public async Task<bool> DeleteIdeaAsync(string ideaId, string userId)
    {
        _entityValidator.ValidateStringField(ideaId, isIdentifier: true);
        _entityValidator.ValidateStringField(userId, isIdentifier: true);
        var idea = await _repositoryIdea.FirstOrDefaultAsync(idea => idea.Id == ideaId && idea.UserId == userId);
        _entityValidator.ValidateEntityIsNotNull(idea);
        await _repositoryIdea.DeleteAsync(idea);
        return true;
    }
    
    public async Task<PaginationView<IdeaPagination>> GetPageAsync(int pageNumber, int pageSize, string identifier = null)
    {
        return await _repositoryIdea.GetPagedAsync(pageNumber, pageSize, identifier);
    }
    
    /// <summary>
    /// Validates the input data for creating or updating a topic.
    /// This method is unique for this service, since the validation is only unique for this topic
    /// </summary>
    /// <param name="ideaDto">The DTO containing the idea data to validate.</param>
    /// <param name="userId">The ID of the user associated with the topic.</param>
    /// <param name="topicId">The DTO containing the topic data to validat</param>
    /// <param name="isUpdate">Indicates whether the validation is for an update operation. Default is false.</param>
    /// <exception cref="ValidationException">Thrown when the validation of the topic fails.</exception>
    /// <exception cref="NullReferenceException">Thrown when the userId is null or empty.</exception>
    private void ValidateInputs(IdeaDto ideaDto, string userId, bool isUpdate = false)
    {
        var ideaValidation = isUpdate
            ? _ideaValidator.Validate(ideaDto, options => options.IncludeRuleSets("update"))
            : _ideaValidator.Validate(ideaDto);

        if (!ideaValidation.IsValid)
        {
            throw new ValidationException(ideaValidation.Errors);
        }

        _entityValidator.ValidateStringField(userId, isIdentifier: true);
    }
}