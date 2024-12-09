using Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;

namespace Persistence.Context;

[ExcludeFromCodeCoverage]
public class ApplicationContext
{
    private IMongoDatabase _database;
    private readonly Dictionary<Type, string> _collectionNames = new Dictionary<Type, string>
    {
        { typeof(Idea), "ideas" },
        { typeof(User), "users" },
        { typeof(Topic), "topics" },
        { typeof(Vote), "votes" }
    };
    
    public ApplicationContext(IMongoDatabase database) 
    {
        _database = database;
        Users = _database.GetCollection<User>(_collectionNames[typeof(User)]);
        Topics = _database.GetCollection<Topic>(_collectionNames[typeof(Topic)]);
        Ideas = _database.GetCollection<Idea>(_collectionNames[typeof(Idea)]);
        Votes = _database.GetCollection<Vote>(_collectionNames[typeof(Vote)]);
        
    }

    public IMongoCollection<User> Users;
    public IMongoCollection<Topic> Topics;
    public IMongoCollection<Idea> Ideas;
    public IMongoCollection<Vote> Votes;

    public IMongoCollection<T> Set<T>()
    {
        return _database.GetCollection<T>(_collectionNames[typeof(T)]);
    }

}