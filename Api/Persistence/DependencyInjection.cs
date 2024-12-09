using Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repository;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using Persistence.Context;

namespace Persistence;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    private const string ConnectionString = "CONNECTION_STRING";
    private const string Database = "DATABASE_MONGO";

    public static void AddPersistence(this IServiceCollection service, IConfiguration configuration)
    {
        var connection = Environment.GetEnvironmentVariable(ConnectionString);
        var database = Environment.GetEnvironmentVariable(Database);
        var mongoClient = new MongoClient(connection);
        service.AddSingleton<IMongoDatabase>(option => mongoClient.GetDatabase(database));
        service.AddScoped<ApplicationContext>();
        
        service.AddScoped(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>));
        service.AddScoped<ITopicRepository, TopicRepository>();
        service.AddScoped<IVoteRepository, VoteRepository>();
        service.AddScoped<IIdeaRepository, IdeaRepository>();
    }
    
}