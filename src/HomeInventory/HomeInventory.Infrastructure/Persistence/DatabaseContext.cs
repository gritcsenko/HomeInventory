using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class DatabaseContext : DbContext, IDatabaseContext, IUnitOfWork
{
    private readonly PublishDomainEventsInterceptor _interceptor;
    private readonly IDateTimeService _dateTimeService;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, PublishDomainEventsInterceptor interceptor, IDateTimeService dateTimeService)
        : base(options)
    {
        _interceptor = interceptor;
        _dateTimeService = dateTimeService;
    }

    public required DbSet<OutboxMessage> OutboxMessages { get; init; }

    public required DbSet<UserModel> Users { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new UserModelConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_interceptor);
        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var now = _dateTimeService.UtcNow;
        foreach (var entry in ChangeTracker.Entries<ICreationAuditableModel>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(x => x.CreatedOn).CurrentValue = now;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<IModificationAuditableModel>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Property(x => x.ModifiedOn).CurrentValue = now;
                    break;
            }
        }
    }
}
