using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using WebApi.Controllers;
using WebApi.Security;
using WebApi.Security.Exceptions;

namespace TestApi.ControllerTest
{
    [TestFixture]
    public class TopicControllerTests
    {
        private Mock<ITopicService> _mockTopicService;
        private Mock<IIdeaService> _mockIdeaService;
        private TopicController _controller;
        private UserAuthorizationMiddleware<Topic, string> _authorizationMiddleware;
        private Mock<IAuthorizationService> _authorizationServiceMock;
        private Mock<IRepositoryAsync<Topic, string>> _repositoryMock;
        private const string IdUser = "09d3ac79-2e5a-4dca-92c8-d6ed56c4cc67";
        private ClaimsPrincipal user;

        [SetUp]
        public void SetUp()
        {
            _mockTopicService = new Mock<ITopicService>();
            _mockIdeaService = new Mock<IIdeaService>();
            _authorizationServiceMock = new Mock<IAuthorizationService>();
            _repositoryMock = new Mock<IRepositoryAsync<Topic, string>>();
            _authorizationMiddleware = 
                new UserAuthorizationMiddleware<Topic, string>(_authorizationServiceMock.Object, _repositoryMock.Object);
            _controller = new TopicController(_mockTopicService.Object, _authorizationMiddleware, _mockIdeaService.Object);
            
            user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, IdUser)
            }));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task CreateTopic_ShouldReturnOkResult()
        {
            var topicDto = new TopicDto { Id = Guid.NewGuid().ToString(), Title = "Test Topic" };
            _mockTopicService.Setup(s => s.CreateTopicAsync(It.IsAny<TopicDto>(), IdUser))
                             .ReturnsAsync(topicDto);

            var result = await _controller.CreateTopic(topicDto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.Value, Is.EqualTo(topicDto));
        }

        [Test]
        public async Task UpdateTopic_ShouldReturnOkResult()
        {
            var topicId = Guid.NewGuid().ToString();
            var updatedTopicDto = new TopicDto { Id = topicId, Title = "Updated Topic" };
            var topicEntity = new Topic { Id = topicId, Title = "Updated Topic", UserId = IdUser };
            
            _mockTopicService.Setup(s => s.UpdateTopicAsync(It.IsAny<TopicDto>(), IdUser))
                             .ReturnsAsync(updatedTopicDto);

            _mockTopicService.Setup(s => s.GetTopicByIdAsync(topicId))
                .ReturnsAsync(updatedTopicDto);
            
            _authorizationServiceMock.Setup(s => 
                    s.AuthorizeAsync(user, topicEntity, It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());
            
            _repositoryMock.Setup(s => 
                    s.FindByIdAsync(topicId))
                .ReturnsAsync(topicEntity);

            var result = await _controller.UpdateTopic(topicId, updatedTopicDto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.Value, Is.EqualTo(updatedTopicDto));
        }
        
        [Test]
        public void UpdateTopic_WhenUserAuthorizationFails()
        {
            var topicId = Guid.NewGuid().ToString();
            var updatedTopicDto = new TopicDto { Id = topicId, Title = "Updated Topic" };
            var topicEntity = new Topic { Id = topicId, Title = "Updated Topic", UserId = Guid.NewGuid().ToString() };        

            _mockTopicService.Setup(s => s.GetTopicByIdAsync(topicId))
                .ReturnsAsync(updatedTopicDto);
            
            _authorizationServiceMock.Setup(s => 
                    s.AuthorizeAsync(user, topicEntity, It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Failed());
            
            _repositoryMock.Setup(s => 
                    s.FindByIdAsync(topicId))
                .ReturnsAsync(topicEntity);

            Assert.ThrowsAsync<UserNotAuthorizedException>(() => _controller.UpdateTopic(topicId, updatedTopicDto));
        }
        
        [Test]
        public void UpdateTopic_ShouldThrowEntityNullException_WhenEntityIsNotFound()
        {
            var topicId = Guid.NewGuid().ToString();
            var updatedTopicDto = new TopicDto { Id = topicId, Title = "Updated Topic" };
            
            _repositoryMock.Setup(s => s.FindByIdAsync(topicId))!
                .ReturnsAsync((Topic)null);

            // Act & Assert
            Assert.ThrowsAsync<EntityNullException>(async () =>
                await _controller.UpdateTopic(topicId, updatedTopicDto));
        }

        [Test]
        public async Task DeleteTopic_ShouldReturnOkResult_WithSuccessMessage()
        {
            var topicId = Guid.NewGuid().ToString();
            var topicToBeDeletedDto = new TopicDto { Id = topicId, Title = "Updated Topic" };
            var topicEntity = new Topic { Id = topicId, Title = "Updated Topic", UserId = IdUser };
            
            _mockTopicService.Setup(s => s.DeleteTopicAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            
            _mockTopicService.Setup(s => s.GetTopicByIdAsync(topicId))
                .ReturnsAsync(topicToBeDeletedDto);
            
            _authorizationServiceMock.Setup(s => 
                    s.AuthorizeAsync(user, topicEntity, It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());
            
            _repositoryMock.Setup(s => 
                    s.FindByIdAsync(topicId))
                .ReturnsAsync(topicEntity);

            var result = await _controller.DeleteTopic(topicId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(200));
                Assert.That(result.Value.ToString(), Is.EqualTo("{ message = Topic successfully deleted. }"));
            });
        }
        
        [Test]
        public void DeleteTopic_WhenUserAuthorizationFails()
        {
            // Arrange
            var topicId = Guid.NewGuid().ToString();
            var topicToBeDeletedDto = new TopicDto { Id = topicId, Title = "Updated Topic" };
            var topicEntity = new Topic { Id = topicId, Title = "Updated Topic", UserId = Guid.NewGuid().ToString() };
            
            _mockTopicService.Setup(s => s.DeleteTopicAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            
            _mockTopicService.Setup(s => s.GetTopicByIdAsync(topicId))
                .ReturnsAsync(topicToBeDeletedDto);
            
            _authorizationServiceMock.Setup(s => 
                    s.AuthorizeAsync(user, topicEntity, It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Failed());
            
            _repositoryMock.Setup(s => 
                    s.FindByIdAsync(topicId))
                .ReturnsAsync(topicEntity);

            // Act
            Assert.ThrowsAsync<UserNotAuthorizedException>(() => _controller.DeleteTopic(topicId));
        }
        
        [Test]
        public void DeleteTopic_ShouldThrowEntityNullException_WhenEntityIsNotFound()
        {
            var topicId = Guid.NewGuid().ToString();
            
            _repositoryMock.Setup(s => s.FindByIdAsync(topicId))!
                .ReturnsAsync((Topic)null);

            // Act & Assert
            Assert.ThrowsAsync<EntityNullException>(async () =>
                await _controller.DeleteTopic(topicId));
        }


        [Test]
        public async Task GetTopicById_ShouldReturnOkResult()
        {
            var topicDto = new TopicDto { Id = Guid.NewGuid().ToString(), Title = "Test Topic" };
            _mockTopicService.Setup(s => s.GetTopicByIdAsync(It.IsAny<string>()))
                             .ReturnsAsync(topicDto);

            var result = await _controller.GetTopicById(topicDto.Id) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.Value, Is.EqualTo(topicDto));
        }

        [Test]
        public async Task GetTopicsByUser_ShouldReturnOkResult()
        {
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
            
            
            _mockTopicService.Setup(s => s.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(pagination);

            var result = await _controller.GetTopicsByUser(1, 2) as OkObjectResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetListTopics_ShouldReturnOkResult()
        {
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
            var topicDtos = new[] { new TopicDto { Id = Guid.NewGuid().ToString(), Title = "Topic 1" } }.AsQueryable();
            _mockTopicService.Setup(s => s.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), null)).ReturnsAsync(pagination);

            var result = await _controller.GetListTopics(1,2) as OkObjectResult;

            Assert.IsNotNull(result);
            
        }
        
        [Test]
        public async Task CreateIdea_ShouldReturnOkResult()
        {
            var ideaDto = new IdeaDto { Id = Guid.NewGuid().ToString(), Description = "Test Idea" };
            var topicId = Guid.NewGuid().ToString();
            var topic = new Topic()
            {
                Id = topicId,
                Title = "Test Topic",
                UserId = IdUser
            };
            
            _repositoryMock
                .Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(topic);
            
            _mockIdeaService.Setup(s => s.CreateIdeaAsync(It.IsAny<IdeaDto>(), IdUser, topicId))
                .ReturnsAsync(ideaDto);
            
            _authorizationServiceMock
                .Setup(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), 
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success);

            var result = await _controller.CreateIdea(ideaDto, topicId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.Value, Is.EqualTo(ideaDto));
        }

        [Test]
        public async Task GetIdeasByTopic_ShouldReturnOkResult()
        {
            var ideas = new List<IdeaPagination>
            {
                new IdeaPagination { CreatedBy = "UserTest", Description = "Idea 1", Votes = 2, Identifier = "idea-1"},
                new IdeaPagination { CreatedBy = "UserTest", Description = "Idea 2", Votes = 5, Identifier = "idea-2"}
            };
            var pagination = new PaginationView<IdeaPagination>()
            {
                PageContent = ideas,
                CurrentPage = 1,
                FoundItems = 2,
                PageAvailable = 1,
                QuantityPerPage = 2
            };
            _mockIdeaService.Setup(s => s.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(pagination);

            var result = await _controller.GetIdeasByTopic(1, 2, "topicId") as OkObjectResult;

            Assert.IsNotNull(result);
        }
    }
}
