using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repository;

namespace TestApi.RepositoryTest;

[TestFixture]
public class IdeaRepositoryTest
{
    private DbContextOptions<ApplicationContext> _options;
    private IdeaRepository _ideaRepository;

    [SetUp]
    public void Setup()
    {
         _options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var context = new ApplicationContext(_options);
        
        context.Users.AddRange(
            new User { Id = "user1", UserName = "User One", Email = "user1@email.com", Password = "password" },
            new User { Id = "user2", UserName = "User Two", Email = "user2@email.com", Password = "password" }
        );

        context.Ideas.AddRange(
            new Idea { Id = "1", TopicId = "topic1", UserId = "user1", Description = "First idea" },
            new Idea { Id = "2", TopicId = "topic1", UserId = "user2", Description = "Second idea" }
        );

        context.Votes.AddRange(
            new Vote { IdeaId = "1", UserId = "user1"},
            new Vote { IdeaId = "1", UserId = "user2" },
            new Vote { IdeaId = "2", UserId = "user1" }
        );

        context.SaveChanges();
        
        _ideaRepository = new IdeaRepository(context);
    }

    [Test]
    public async Task GetPaginatedIdeasByTopicAsync_ReturnsCorrectResults()
    {
        int page = 1;
        int pageSize = 2;
        string topicId = "topic1";
        
        var ideaPagination = await _ideaRepository.GetPagedAsync(page, pageSize, topicId);
        var count = ideaPagination.PageContent.Count();
        Assert.That(count, Is.EqualTo(2));
        Assert.That(ideaPagination.PageContent.First().Identifier, Is.EqualTo("1"));
        Assert.That(ideaPagination.PageContent.First().CreatedBy, Is.EqualTo("User One"));
        Assert.That(ideaPagination.PageContent.First().Description, Is.EqualTo("First idea"));
        Assert.That(ideaPagination.PageContent.First().Votes, Is.EqualTo(2));
        Assert.That(ideaPagination.CurrentPage, Is.EqualTo(1));
        Assert.That(ideaPagination.QuantityPerPage, Is.EqualTo(2));
        Assert.That(ideaPagination.FoundItems, Is.EqualTo(2));
        Assert.That(ideaPagination.PageAvailable, Is.EqualTo(1));
    }
}