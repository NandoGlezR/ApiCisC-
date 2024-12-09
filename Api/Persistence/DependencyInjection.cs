using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Persistence.Repository;
using System.Diagnostics.CodeAnalysis;

namespace Persistence;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    private const string ConnectionString = "ConnectionServer";

    public static void AddPersistence(this IServiceCollection service, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString(ConnectionString);
        service.AddDbContext<ApplicationContext>(option =>
        {
            if (connection != null) option.UseMySQL(connection, builder => builder.MigrationsAssembly("Persistence"));
        });
        service.AddScoped(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>));
        service.AddScoped<ITopicRepository, TopicRepository>();
        service.AddScoped<IVoteRepository, VoteRepository>();
        service.AddScoped<IIdeaRepository, IdeaRepository>();
    }

    public static void AddMigrations(this IServiceProvider serviceProvider)
    {
        var dbContextOption = serviceProvider.GetRequiredService<DbContextOptions<ApplicationContext>>();
        using var dbContext = new ApplicationContext(dbContextOption);
        dbContext.Database.Migrate();
    }
}