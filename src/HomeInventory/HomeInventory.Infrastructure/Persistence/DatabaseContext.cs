using System.Linq.Expressions;
using DotNext;
using DotNext.Collections.Generic;
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
        var now = _dateTimeService.UtcNow;
        UpdateAuditableEntities(now);

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public Optional<TEntity> FindTracked<TEntity>(Predicate<TEntity> condition)
        where TEntity : class
    {
        return ChangeTracker.Entries<TEntity>().Select(e => e.Entity).FirstOrNone(condition);
    }

    private void UpdateAuditableEntities(DateTimeOffset now)
    {
        UpdateTimeAuditEntities<IHasCreationAudit>(now, EntityState.Added, x => x.CreatedOn);
        UpdateTimeAuditEntities<IHasModificationAudit>(now, EntityState.Modified, x => x.ModifiedOn);
    }

    private void UpdateTimeAuditEntities<TEntity>(DateTimeOffset now, EntityState state, Expression<Func<TEntity, DateTimeOffset>> propertyExpression)
        where TEntity : class
    {
        foreach (var entry in ChangeTracker.Entries<TEntity>())
        {
            if (entry.State == state)
            {
                var propertyEntry = entry.Property(propertyExpression);
                propertyEntry.CurrentValue = now;
            }
        }
    }
}
