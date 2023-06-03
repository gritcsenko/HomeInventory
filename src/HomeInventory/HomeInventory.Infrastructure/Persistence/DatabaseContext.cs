using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class DatabaseContext : DbContext, IDatabaseContext, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    public required DbSet<UserModel> Users { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserModelConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<IAggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(root => root.DomainEvents.Count > 0)
            .SelectMany(root => root.DomainEvents);

        var savedCount = await base.SaveChangesAsync(cancellationToken);
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return savedCount;
    }
}
