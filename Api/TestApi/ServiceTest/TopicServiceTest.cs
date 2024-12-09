using System.Linq.Expressions;
using Application.Dto;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Repository;
using Domain.Views;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace TestApi.ServiceTest
{
    [TestFixture]
    public class TopicServiceTest
    {
        private Mock<ITopicRepository> _mockRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IValidator<TopicDto>> _mockValidator;
        private Mock<IEntityValidator> _entityValidatorMock;
        private TopicService _topicService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ITopicRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockValidator = new Mock<IValidator<TopicDto>>();
            _entityValidatorMock = new Mock<IEntityValidator>();
            _topicService = new TopicService(_mockRepository.Object, _mockMapper.Object, _mockValidator.Object,
                _entityValidatorMock.Object);
        }

        [Test]
        public async Task CreateTopicAsync_ShouldCreateTopic_WhenInputsAreValid()
        {
            var topicDto = new TopicDto { Title = "New Topic" };
            var topic = new Topic { Id = topicDto.Id, Title = "New Topic", UserId = "user-123" };

            _mockValidator.Setup(v => v.Validate(It.IsAny<TopicDto>()))
                .Returns(new ValidationResult());

            _mockMapper.Setup(m => m.Map<Topic>(It.IsAny<TopicDto>())).Returns(topic);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Topic>())).Returns(Task.CompletedTask);
        
            var result = await _topicService.CreateTopicAsync(topicDto, "user-123");

            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Topic>()), Times.Once);
            Assert.That(result.Id, Is.EqualTo(topicDto.Id));
        }

        [Test]
        public void CreateTopicAsync_ShouldThrowValidationException_WhenTopicDtoIsInvalid()
        {
            var topicDto = new TopicDto { Title = "" };

            _mockValidator.Setup(v => v.Validate(It.IsAny<TopicDto>()))
                .Returns(new ValidationResult(new[]
                {
                    new ValidationFailure("Title", "Title cannot be empty")
                }));
        
            Assert.ThrowsAsync<ValidationException>(() => _topicService.CreateTopicAsync(topicDto, "user-123"));
        }
        
        [Test]
        public async Task DeleteTopicAsync_ShouldDeleteTopic_WhenExists()
        {
            // Arrange
            var topic = new Topic { Id = "topic-123", Title = "Test", UserId = "user-123" };

            _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Topic, bool>>>()))
                .ReturnsAsync(topic);

            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Topic>())).Returns(Task.CompletedTask);

            // Act
            var result = await _topicService.DeleteTopicAsync("topic-123", "user-123");

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Topic>()), Times.Once);
            Assert.IsTrue(result);
        }
        
        [Test]
        public async Task GetPagedTopicsByUserAsync_ShouldReturnTopics_WhenUserIdIsValid()
        {
            var userId = "user-123";
            var topics = new List<TopicPagination>
            {
                new TopicPagination { CreatedBy = "UserTest", Title = "Topic 1", Identifier = "topic-1"},
                new TopicPagination { CreatedBy = "UserTest", Title = "Topic 2", Identifier = "topic-2"}
            };
            var pagination = new PaginationView<TopicPagination>()
            {
                PageContent = topics,
                CurrentPage = 1,
                FoundItems = 2,
                PageAvailable = 1,
                QuantityPerPage = 2
            };

            _mockRepository.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(pagination);
            
            var result = await _topicService.GetPageAsync(1,2,userId);
            
            Assert.That(result.FoundItems, Is.EqualTo(2));
            Assert.That(result.PageAvailable, Is.EqualTo(1));
            Assert.That(result.QuantityPerPage, Is.EqualTo(2));
            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.PageContent.First().Title, Is.EqualTo("Topic 1"));
        }
        
        [Test]
        public async Task GetTopicByIdAsync_ShouldReturnTopic_WhenTopicIdIsValid()
        {
            var topicId = "topic-123";
            var topic = new Topic { Id = topicId, Title = "Test Topic", UserId = "user-123"};

            _mockRepository.Setup(r => r.FindByIdAsync(topicId))
                .ReturnsAsync(topic);

            _mockMapper.Setup(m => m.Map<TopicDto>(topic))
                .Returns(new TopicDto { Id = topicId, Title = "Test Topic" });
            
            var result = await _topicService.GetTopicByIdAsync(topicId);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(topicId));
        }
        
        [Test]
        public async Task GetListTopicsAsync_ShouldReturnAllTopics()
        {
            var topics = new List<TopicPagination>
            {
                new TopicPagination { CreatedBy = "UserTest", Title = "Topic 1", Identifier = "topic-1"},
                new TopicPagination { CreatedBy = "UserTest7", Title = "Topic 2", Identifier = "topic-2"},
                new TopicPagination { CreatedBy = "UserTest2", Title = "Topic 3", Identifier = "topic-3"},
                new TopicPagination { CreatedBy = "UserTest3", Title = "Topic 4", Identifier = "topic-4"}
            };
            var pagination = new PaginationView<TopicPagination>()
            {
                PageContent = topics,
                CurrentPage = 2,
                FoundItems = 4,
                PageAvailable = 2,
                QuantityPerPage = 2
            };

            _mockRepository.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(pagination);
            
            var result = await _topicService.GetPageAsync(2,2);
            
            Assert.That(result.FoundItems, Is.EqualTo(4));
            Assert.That(result.PageAvailable, Is.EqualTo(2));
            Assert.That(result.QuantityPerPage, Is.EqualTo(2));
            Assert.That(result.CurrentPage, Is.EqualTo(2));
            Assert.That(result.PageContent.First().Title, Is.EqualTo("Topic 1"));
        }
    }
}