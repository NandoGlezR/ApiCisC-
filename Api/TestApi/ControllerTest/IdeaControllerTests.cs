using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Domain.Entities;
using Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using WebApi.Controllers;
using WebApi.Security;

namespace TestApi.ControllerTest
{
    [TestFixture]
    public class IdeaControllerTests
    {
        private Mock<IIdeaService> _mockIdeaService;
        private Mock<IVoteService> _mockVoteService;
        private Mock<IAuthorizationService> _mockAuthorizationService;
        private Mock<IRepositoryAsync<Idea, string>> _mockIdeaRepository;
        private Mock<IVoteRepository> _mockVoteRepository;
        private UserAuthorizationMiddleware<Idea, string> _ideaAuthorizationMiddleware;
        private UserAuthorizationMiddleware<Vote, string> _voteAuthorizationMiddleware;
        private IdeaController _controller;
        private const string IdUser = "09d3ac79-2e5a-4dca-92c8-d6ed56c4cc67";
        private const string IdTopic = "00000000-2e5a-4dca-92c8-d6ed56c4cc67";

        [SetUp]
        public void SetUp()
        {
            _mockIdeaService = new Mock<IIdeaService>();
            _mockVoteService = new Mock<IVoteService>();
            
            _mockAuthorizationService = new Mock<IAuthorizationService>();
            _mockIdeaRepository = new Mock<IRepositoryAsync<Idea, string>>();
            _mockVoteRepository = new Mock<IVoteRepository>();

            _ideaAuthorizationMiddleware = new UserAuthorizationMiddleware<Idea, string>(
                _mockAuthorizationService.Object, _mockIdeaRepository.Object);
            
            _voteAuthorizationMiddleware = new UserAuthorizationMiddleware<Vote, string>(
                _mockAuthorizationService.Object, _mockVoteRepository.Object);
            
            _controller = new IdeaController(_mockIdeaService.Object, _mockVoteService.Object, _mockVoteRepository.Object, 
                 _ideaAuthorizationMiddleware,  _voteAuthorizationMiddleware);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, IdUser)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
        
        [Test]
        public async Task UpdateIdea_ShouldReturnOkResult()
        {
            var ideaId = Guid.NewGuid().ToString();
            var ideaEntity = new Idea
            {
                Id = ideaId,
                Description = "Idea1",
                TopicId = IdTopic,
                UserId = IdUser
            };
            var updatedIdeaDto = new IdeaDto { Id = ideaId, Description = "Updated Idea1" };
            
            _mockIdeaRepository
                .Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(ideaEntity);

            _mockAuthorizationService
                .Setup(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(),
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success);
            
            _mockIdeaService
                .Setup(s => s.UpdateIdeaAsync(It.IsAny<IdeaDto>(), IdUser))
                .ReturnsAsync(updatedIdeaDto);
            
            var result = await _controller.UpdateIdea(ideaId, updatedIdeaDto) as OkObjectResult;
            
            Assert.IsNotNull(result);
            Assert.That(result.Value, Is.EqualTo(updatedIdeaDto));
        }

        [Test]
        public async Task DeleteIdea_ShouldReturnOkResult_WithSuccessMessage()
        {
            var ideaId = Guid.NewGuid().ToString();
            var ideaEntity = new Idea
            {
                Id = ideaId,
                Description = "Description idea",
                TopicId = IdTopic,
                UserId = IdUser
            };
            
            _mockIdeaRepository
                .Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(ideaEntity);
            
            _mockAuthorizationService
                .Setup(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(),
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success);

            _mockIdeaService.Setup(s => s.DeleteIdeaAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await _controller.DeleteIdea(ideaId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task CreateVote_ShouldReturnOkResult_WhenVoteIsCreated()
        {
            var ideaId = Guid.NewGuid().ToString();
            var voteCreated = true; 
            var ideaEntity = new Idea
            {
                Id = ideaId,
                Description = "Description idea",
                TopicId = IdTopic,
                UserId = IdUser
            };
            
            _mockIdeaRepository
                .Setup(repo => repo.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(ideaEntity);
            
            _mockAuthorizationService
                .Setup(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(),
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success);
            
            _mockVoteService.Setup(s => s.CreateVoteAsync(ideaId, IdUser))
                .ReturnsAsync(voteCreated);
            
            var result = await _controller.CreateVote(ideaId) as OkObjectResult;
            
            Assert.IsNotNull(result);
            Assert.That(result.Value, Is.EqualTo(voteCreated));
        }
        
        [Test]
        public async Task DeleteVote_ShouldReturnOkResult_WhenVoteIsDeleted()
        {
            var ideaId = Guid.NewGuid().ToString();  
            var voteDeleted = true;
            var userId = IdUser;
            var voteId = Guid.NewGuid().ToString();
            var existingVote = new Vote
            {
                Id = voteId,
                IdeaId = ideaId,
                UserId = userId
            };

            _mockVoteRepository
                .Setup(repo => repo.FindByIdAsync(voteId))
                .ReturnsAsync(existingVote); 
            
            _mockVoteRepository
                .Setup(repo => repo.FirstOrDefaultAsync(vote => vote.IdeaId == ideaId && vote.UserId == userId))
                .ReturnsAsync(existingVote); 
            
            _mockAuthorizationService
                .Setup(s => s.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(),
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success); 
            
            _mockVoteService.Setup(s => s.DeleteVoteAsync(ideaId, IdUser))
                .ReturnsAsync(voteDeleted);

            var result = await _controller.DeleteVote(ideaId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.That(result.Value, Is.EqualTo(true));
        }
    }
}
