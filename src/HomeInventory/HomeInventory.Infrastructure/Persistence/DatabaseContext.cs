using System.Linq.Expressions;
using DotNext;
using DotNext.Collections.Generic;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class DatabaseContext : DbContext, IDatabaseContext, IUnitOfWork
{
    private readonly PublishDomainEventsInterceptor _interceptor;
    private readonly IDateTimeService _dateTimeService;
    private readonly IEnumerable<IDatabaseConfigurationApplier> _configurationAppliers;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, PublishDomainEventsInterceptor interceptor, IDateTimeService dateTimeService, IEnumerable<IDatabaseConfigurationApplier> configurationAppliers)
        : base(options)
    {
        _interceptor = interceptor;
        _dateTimeService = dateTimeService;
        _configurationAppliers = configurationAppliers;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var applier in _configurationAppliers)
        {
            applier.ApplyConfigurationTo(modelBuilder);
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_interceptor);

        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(_dateTimeService.UtcNow);

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public Optional<TEntity> FindTracked<TEntity>(Predicate<TEntity> condition)
        where TEntity : class =>
        ChangeTracker.Entries<TEntity>().Select(e => e.Entity).FirstOrNone(condition);

    public DbSet<TEntity> GetDbSet<TEntity>()
        where TEntity : class =>
        Set<TEntity>();

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
