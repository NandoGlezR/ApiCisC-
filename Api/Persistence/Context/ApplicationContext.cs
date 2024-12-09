using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Persistence.Context;

[ExcludeFromCodeCoverage]
public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> option) : base(option)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Idea> Ideas { get; set; }
    public DbSet<Vote> Votes {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vote>().HasKey(key => new {key.IdeaId, key.UserId});
    }
}