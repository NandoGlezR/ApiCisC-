using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repository;

namespace TestApi.RepositoryTest;

[TestFixture]
public class TopicRepositoryTest
{
    private DbContextOptions<ApplicationContext> _options;
    private TopicRepository _topicRepository;
    private ApplicationContext _context;
    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
       _context = new ApplicationContext(_options);
        
        _context.Users.AddRange(
            new User { Id = "user1", UserName = "User One", Email = "user1@email.com", Password = "password" },
            new User { Id = "user2", UserName = "User Two", Email = "user2@email.com", Password = "password" }
        );

        _context.Topics.AddRange(
            new Topic { Id = "topic1", UserId = "user1", Title = "First Topic" },
            new Topic { Id = "topic2", UserId = "user2", Title = "Second Topic" }
        );

        _context.SaveChanges();
        
        _topicRepository = new TopicRepository(_context);
    }

    [Test]
    public async Task GetTopicsPagedAsync_ReturnsCorrectResults_WhenNoUserIdProvided()
    {
        int page = 0;
        int pageSize = 0;
        
        var topicPagination = await _topicRepository.GetPagedAsync(page, pageSize);
        var count = topicPagination.PageContent.Count();
        Assert.That(count, Is.EqualTo(2));
        Assert.That(topicPagination.PageContent.First().CreatedBy, Is.EqualTo("User One"));
        Assert.That(topicPagination.PageContent.First().Title, Is.EqualTo("First Topic"));
    }

    [Test]
    public async Task GetTopicsPagedAsync_ReturnsCorrectResults_WhenUserIdIsProvided()
    {
        int page = 1;
        int pageSize = 2;
        string userId = "user1";
        
        var topicPagination = await _topicRepository.GetPagedAsync(page, pageSize, userId);
        var count = topicPagination.PageContent.Count();
        Assert.That(count, Is.EqualTo(1));
        Assert.That(topicPagination.PageContent.First().CreatedBy, Is.EqualTo("User One"));
        Assert.That(topicPagination.PageContent.First().Title, Is.EqualTo("First Topic"));
    }
    
    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}