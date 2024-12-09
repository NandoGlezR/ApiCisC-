using System.Linq.Expressions;
using Application.Core.Mapper;
using Application.Core.Validations;
using Application.Dto;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Repository;
using Domain.Views;
using Moq;


namespace TestApi.ServiceTest;

[TestFixture]
public class IdeaServiceTest
{
    private Mock<IIdeaRepository> _mockRepository;
    private Mock<IEntityValidator> _entityValidatorMock;
    private IdeaService _ideaService;
    private IMapper _mapper;

    private const string IdUser = "550e8400-e29b-41d4-a716-446655440000";

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IIdeaRepository>();
        var mappingConfig = new MapperConfiguration(cfg => { cfg.AddProfile<ManagerMapper>(); });

        _mapper = mappingConfig.CreateMapper();

        _ideaService = new IdeaService(
            _mockRepository.Object,
            _mapper,
            new IdeaDtoValidator(),
            new EntityValidator(new StringValidator())
        );
    }

    [Test]
    public async Task CreateIdeaAsync_ShouldCreateIdea_WithInputsSucessfull()
    {
        var ideaDto = new IdeaDto { Description = "This is a new idea" };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Idea>())).Returns(Task.CompletedTask);

        var result = await _ideaService.CreateIdeaAsync(ideaDto, IdUser, "Topic1");

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Idea>()), Times.Once);
        Assert.That(result.Id, Is.EqualTo(ideaDto.Id));
    }
    
    [Test]
    public async Task UpdateIdeaAsync_ShouldUpdateIdea_WithInputsSucessfull()
    {
        var ideaDto = new IdeaDto { Description = "This is a description" };
        var idea = new Idea
        {
            Id = ideaDto.Id,
            Description = "This is a description",
            UserId = IdUser,
            TopicId = "Topic1"
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Idea>())).Returns(Task.CompletedTask);

        var result = await _ideaService.CreateIdeaAsync(ideaDto, IdUser, "Topic1");

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Idea>()), Times.Once);
        Assert.That(result.Id, Is.EqualTo(ideaDto.Id));

        ideaDto.Description = "This is a new description";

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Idea>())).Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Idea, bool>>>())).ReturnsAsync(idea);
        var resultNew = await _ideaService.UpdateIdeaAsync(ideaDto, IdUser);

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Idea>()), Times.Once);
        Assert.That(resultNew.Id, Is.EqualTo(ideaDto.Id));
        Assert.That(resultNew.Description, Is.EqualTo("This is a new description"));
    }

    [Test]
    public async Task CreateIdeaAsync_ShouldThrowException_WhenTopicIsMissing()
    {
        var ideaDto = new IdeaDto { Description = "This is a new idea" };

        Assert.ThrowsAsync<ArgumentNullException>(() => _ideaService.CreateIdeaAsync(ideaDto, "user1", null));
    }

    [Test]
    public async Task GetPagedIdeaAsync_ShouldReturnTopics_WhenUserIdIsValid()
    {
        var userId = "user-123";
        var ideas = new List<IdeaPagination>
        {
            new IdeaPagination { Identifier = "topic-1", Description = "Idea 1", Votes = 3, CreatedBy = userId },
            new IdeaPagination { Identifier = "topic-2", Description = "Idea 2", Votes = 2, CreatedBy = userId }
        };
        var pagination = new PaginationView<IdeaPagination>()
        {
            PageContent = ideas,
            CurrentPage = 1,
            FoundItems = 2,
            PageAvailable = 1,
            QuantityPerPage = 2
        };

        _mockRepository.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(pagination);

        var result = await _ideaService.GetPageAsync(1, 2, "topic-1");

        Assert.That(result.FoundItems, Is.EqualTo(2));
        Assert.That(result.PageAvailable, Is.EqualTo(1));
        Assert.That(result.QuantityPerPage, Is.EqualTo(2));
        Assert.That(result.CurrentPage, Is.EqualTo(1));
        Assert.That(result.PageContent.First().Description, Is.EqualTo("Idea 1"));
    }
}