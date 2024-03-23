using System.Linq.Expressions;
using DotNext;
using HomeInventory.Core;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class DatabaseContext(DbContextOptions<DatabaseContext> options, PublishDomainEventsInterceptor interceptor, IDateTimeService dateTimeService, IEnumerable<IDatabaseConfigurationApplier> configurationAppliers) : DbContext(options), IDatabaseContext, IUnitOfWork
{
    private readonly PublishDomainEventsInterceptor _interceptor = interceptor;
    private readonly IDateTimeService _dateTimeService = dateTimeService;
    private readonly IReadOnlyCollection<IDatabaseConfigurationApplier> _configurationAppliers = configurationAppliers.ToArray();

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(_dateTimeService.UtcNow);

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public Optional<TEntity> FindTracked<TEntity>(Func<TEntity, bool> condition)
        where TEntity : class =>
        ChangeTracker.Entries<TEntity>().Select(e => e.Entity).FirstOrNone(condition);

    public DbSet<TEntity> GetDbSet<TEntity>()
        where TEntity : class =>
        Set<TEntity>();

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

    private void UpdateAuditableEntities(DateTimeOffset now)
    {
        UpdateTimeAuditEntities<IHasCreationAudit>(now, EntityState.Added, x => x.CreatedOn);
        UpdateTimeAuditEntities<IHasModificationAudit>(now, EntityState.Modified, x => x.ModifiedOn);
    }

    private void UpdateTimeAuditEntities<TEntity>(DateTimeOffset now, EntityState state, Expression<Func<TEntity, DateTimeOffset>> propertyExpression)
        where TEntity : class
    {
        var properties = ChangeTracker
            .Entries<TEntity>()
            .Where(e => e.State == state)
            .Select(e => e.Property(propertyExpression));
        foreach (var property in properties)
        {
            property.CurrentValue = now;
        }
    }
}
