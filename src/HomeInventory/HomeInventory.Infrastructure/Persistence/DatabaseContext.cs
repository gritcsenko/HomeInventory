using System.Linq.Expressions;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Framework.Models.Configuration;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class DatabaseContext(DbContextOptions<DatabaseContext> options, PublishDomainEventsInterceptor interceptor, TimeProvider timeProvider, IEnumerable<IDatabaseConfigurationApplier> configurationAppliers) : DbContext(options), IDatabaseContext, IUnitOfWork
{
    private readonly PublishDomainEventsInterceptor _interceptor = interceptor;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IReadOnlyCollection<IDatabaseConfigurationApplier> _configurationAppliers = configurationAppliers.ToArray();

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(_timeProvider.GetUtcNow());

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public Option<TEntity> FindTracked<TEntity>(Func<TEntity, bool> condition)
        where TEntity : class =>
        ChangeTracker
            .Entries<TEntity>()
            .Select(static e => e.Entity)
            .Where(condition)
            .ToOption();

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
        UpdateTimeAuditEntities<IHasCreationAudit>(now, EntityState.Added, static x => x.CreatedOn);
        UpdateTimeAuditEntities<IHasModificationAudit>(now, EntityState.Modified, static x => x.ModifiedOn);
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
