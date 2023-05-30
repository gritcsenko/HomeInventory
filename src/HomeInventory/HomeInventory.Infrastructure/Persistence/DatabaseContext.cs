using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class DatabaseContext : DbContext, IDatabaseContext, IUnitOfWork
{
    private readonly IDateTimeService _dateTimeService;

    public DatabaseContext(IDateTimeService dateTimeService, DbContextOptions<DatabaseContext> options)
        : base(options)
    {
        _dateTimeService = dateTimeService;
    }

    public required DbSet<OutboxMessage> OutboxMessages { get; init; }

    public required DbSet<UserModel> Users { get; init; }

    public required DbSet<StorageAreaModel> StorageAreas { get; init; }

    public required DbSet<ProductModel> Products { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new UserModelConfiguration());
        modelBuilder.ApplyConfiguration(new StorageAreaModelConfiguration());
        modelBuilder.ApplyConfiguration(new ProductModelConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAmountModelConfiguration());
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ConvertDomainEventsToOutboxMessages();
        UpdateAuditableEntities();

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ConvertDomainEventsToOutboxMessages()
    {
        var messages = base.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(root => root.GetAndClearEvents())
            .Select(CreateMessage);

        OutboxMessages.AddRange(messages);
    }

    private static OutboxMessage CreateMessage(IDomainEvent domainEvent)
    {
        return new OutboxMessage(domainEvent.Id, domainEvent.Created, domainEvent);
    }

    private void UpdateAuditableEntities()
    {
        var now = _dateTimeService.UtcNow;
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(x => x.CreatedOn).CurrentValue = now;
                    break;

                case EntityState.Modified:
                    entry.Property(x => x.ModifiedOn).CurrentValue = now;
                    break;
            }
        }
    }
}
