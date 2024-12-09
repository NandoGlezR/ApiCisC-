using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repository;

namespace TestApi.RepositoryTest;

[TestFixture]
public class RepositoryAsyncTest
{
    private DbContextOptions<ApplicationContext> _options;
    private RepositoryAsync<Topic, string> _repository;
    private const string IdTopic = "aa29b346-880c-4167-b020-77d2eee18469";
    private const string IdTopicUpdate = "8af338c1-ad76-4572-9117-06f0685843a4";
    private const string IdUser = "8fb319e3-1fb8-4ac2-9d82-a84308d26d2a";
    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var context = new ApplicationContext(_options);
        context.Users.AddAsync(new User() { Id = IdUser, Email = "test@gmail.com", Password = "Password", UserName = "Test" });
        _repository = new RepositoryAsync<Topic, string>(context);
    }

    [Test]
    public async Task TestTopicAddAndSaveChanges()
    {
        var topic = new Topic()
        {
            Id = IdTopic,
            Title = "This is a test",
            UserId = IdUser
        };
        await _repository.AddAsync(topic);
        var result = await _repository.FindByIdAsync(topic.Id);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo(topic.Title));
    }

    [Test]
    public async Task TestTopicUpdateAndSaveChanges()
    {
        var topic = new Topic()
        {
            Id = IdTopicUpdate,
            Title = "This is a test updated",
            UserId = IdUser
        };
        await _repository.AddAsync(topic);
        topic.Title = "Updated";
        await _repository.UpdateAsync(topic);
        var result = await _repository.FindByIdAsync(topic.Id);
        var resultFirts = await _repository.FirstOrDefaultAsync(t => t.Id == topic.Id);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo(topic.Title));
        Assert.That(resultFirts, Is.Not.Null);
        Assert.That(resultFirts.UserId, Is.EqualTo(topic.UserId));
    }

    [Test]
    public async Task TestTopicDeleteAndSaveChanges()
    {
        var topic = new Topic()
        {
            Id = IdTopic,
            Title = "This is a test updated",
            UserId = IdUser
        };
        await _repository.AddAsync(topic);
        var result = await _repository.FindByIdAsync(topic.Id);
        Assert.That(result, Is.Not.Null);
        await _repository.DeleteAsync(topic);
        var resultDeleted = await _repository.FindByIdAsync(topic.Id);
        Assert.That(resultDeleted, Is.Null);
    }

    [Test]
    public async Task TestTopicAllListAndSaveChanges()
    {
        var listTopic = new List<Topic>()
        {
            new Topic() { Id = IdTopic, Title = "This is a test One", UserId = IdUser },
            new Topic() { Id = IdTopicUpdate, Title = "This is a test Two", UserId = IdUser },
        };

        foreach (var topic in listTopic)
        {
            await _repository.AddAsync(topic);
        }

        var list = await _repository.GetAllAsync();
        var listAllSearch = await _repository.FindAllAsync(topic => topic.Title == "This is a test One");
        var listAll = list.ToList();
        var allSearch = listAllSearch.ToList();
        Assert.That(allSearch, Is.Not.Null);
        Assert.That(listAll, Is.Not.Null);
        Assert.That(allSearch.Count, Is.EqualTo(1));
        Assert.That(listAll.Count, Is.EqualTo(2));
    }
}