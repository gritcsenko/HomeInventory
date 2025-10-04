using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Framework.Models.Configuration;

public abstract class BaseDatabaseConfigurationApplier<TConfiguration, TEntity>(Func<TConfiguration> configurationFactoryFunc) : IDatabaseConfigurationApplier
    where TConfiguration : class, IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    private readonly Func<TConfiguration> _configurationFactoryFunc = configurationFactoryFunc;

    protected BaseDatabaseConfigurationApplier()
        : this(Activator.CreateInstance<TConfiguration>)
    {
    }

    public void ApplyConfigurationTo(ModelBuilder modelBuilder) => modelBuilder.ApplyConfiguration(_configurationFactoryFunc());
}
