using System.Linq.Expressions;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Repository;
using Moq;

namespace TestApi.ServiceTest;

[TestFixture]
public class VoteServiceTest
{
    private Mock<IVoteRepository> _mockRepository;
    private Mock<IEntityValidator> _mockValidator;
    private VoteService _voteService;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IVoteRepository>();
        _mockValidator = new Mock<IEntityValidator>();
        _voteService = new VoteService(_mockRepository.Object, _mockValidator.Object);
    }

    [Test]
    public async Task CreateVoteAsync_VoteDoesNotExist_ShouldReturnTrue()
    {
        var ideaId = "idea1";
        var userId = "user1";
        _mockValidator.Setup(v => v.ValidateStringField(ideaId, true)).Verifiable();
        _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Vote, bool>>>()))!
                       .ReturnsAsync((Vote)null!);
        
        var result = await _voteService.CreateVoteAsync(ideaId, userId);
        
        Assert.IsTrue(result);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Vote>()), Times.Once);
    }

    [Test]
    public async Task CreateVoteAsync_VoteAlreadyExists_ShouldReturnFalse()
    {
        var ideaId = "idea1";
        var userId = "user1";
        var existingVote = new Vote { Id = "1", IdeaId = ideaId, UserId = userId };

        _mockValidator.Setup(v => v.ValidateStringField(ideaId, true)).Verifiable();
        _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                       .ReturnsAsync(existingVote);
        
        var result = await _voteService.CreateVoteAsync(ideaId, userId);
        
        Assert.IsFalse(result);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Vote>()), Times.Never);
    }

    [Test]
    public async Task DeleteVoteAsync_VoteExists_ShouldReturnTrue()
    {
        var ideaId = "idea1";
        var userId = "user1";
        var existingVote = new Vote { Id = "1", IdeaId = ideaId, UserId = userId };

        _mockValidator.Setup(v => v.ValidateStringField(ideaId, true)).Verifiable();
        _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                       .ReturnsAsync(existingVote);
        
        var result = await _voteService.DeleteVoteAsync(ideaId, userId);
        
        Assert.IsTrue(result);
        _mockRepository.Verify(r => r.DeleteAsync(existingVote), Times.Once);
    }

    [Test]
    public async Task DeleteVoteAsync_VoteDoesNotExist_ShouldReturnFalse()
    {
        var ideaId = "idea1";
        var userId = "user1";

        _mockValidator.Setup(v => v.ValidateStringField(ideaId, true)).Verifiable();
        _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Vote, bool>>>()))
                       .ReturnsAsync((Vote)null);
        
        var result = await _voteService.DeleteVoteAsync(ideaId, userId);
        
        Assert.IsFalse(result);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Vote>()), Times.Never);
    }

    [Test]
    public async Task GetNumberVotesAsync_ShouldReturnCorrectCount()
    {
        var ideaId = "idea1";
        var voteCount = 5;

        _mockValidator.Setup(v => v.ValidateStringField(ideaId, true)).Verifiable();
        _mockRepository.Setup(r => r.CountVote(ideaId)).ReturnsAsync(voteCount);
        
        var result = await _voteService.GetNumberVotesAsync(ideaId);
        
        Assert.That(result, Is.EqualTo(voteCount));
    }
}
