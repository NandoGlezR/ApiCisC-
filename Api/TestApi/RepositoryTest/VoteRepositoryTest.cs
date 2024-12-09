using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repository;

namespace TestApi.RepositoryTest
{
    [TestFixture]
    public class VoteRepositoryTest
    {
        private ApplicationContext _context;
        private VoteRepository _voteRepository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryVoteDb")
                .Options;

            _context = new ApplicationContext(options);
            _voteRepository = new VoteRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CountVote_WhenVotesExistForIdea_ShouldReturnCorrectCount()
        {
            var ideaId = "idea1";
            var votes = new[]
            {
                new Vote { IdeaId = ideaId, UserId = "user1" },
                new Vote { IdeaId = ideaId, UserId = "user2" },
                new Vote { IdeaId = ideaId, UserId = "user3" }
            };

            await _context.Votes.AddRangeAsync(votes);
            await _context.SaveChangesAsync();

            var voteCount = await _voteRepository.CountVote(ideaId);

            Assert.That(voteCount, Is.EqualTo(3));
        }

        [Test]
        public async Task CountVote_WhenNoVotesExistForIdea_ShouldReturnZero()
        {
            var ideaId = "idea1";

            var voteCount = await _voteRepository.CountVote(ideaId);

            Assert.That(voteCount, Is.EqualTo(0));
        }

        [Test]
        public async Task AddAsync_ShouldAddVoteToDatabase()
        {
            var newVote = new Vote { IdeaId = "idea1", UserId = "user1" };

            await _voteRepository.AddAsync(newVote);
            await _context.SaveChangesAsync();

            var savedVote = await _context.Votes.FirstOrDefaultAsync(v => v.IdeaId == "idea1" && v.UserId == "user1");
            Assert.IsNotNull(savedVote);
            Assert.That(savedVote.IdeaId, Is.EqualTo("idea1"));
            Assert.That(savedVote.UserId, Is.EqualTo("user1"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveVoteFromDatabase()
        {
            var vote = new Vote { IdeaId = "idea1", UserId = "user1" };

            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();

            await _voteRepository.DeleteAsync(vote);
            await _context.SaveChangesAsync();

            var deletedVote = await _context.Votes.FirstOrDefaultAsync(v => v.IdeaId == "idea1" && v.UserId == "user1");
            Assert.IsNull(deletedVote);
        }

        [Test]
        public async Task FirstOrDefaultAsync_ShouldReturnVoteIfExists()
        {
            var vote = new Vote { IdeaId = "idea1", UserId = "user1" };

            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();

            var retrievedVote = await _voteRepository.FirstOrDefaultAsync(v => v.IdeaId == "idea1" && v.UserId == "user1");

            Assert.IsNotNull(retrievedVote);
            Assert.That(retrievedVote.IdeaId, Is.EqualTo("idea1"));
            Assert.That(retrievedVote.UserId, Is.EqualTo("user1"));
        }

        [Test]
        public async Task FirstOrDefaultAsync_ShouldReturnNullIfVoteDoesNotExist()
        {
            var ideaId = "idea1";
            var userId = "user1";

            var retrievedVote = await _voteRepository.FirstOrDefaultAsync(v => v.IdeaId == ideaId && v.UserId == userId);

            Assert.IsNull(retrievedVote);
        }
    }
}
