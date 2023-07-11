using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal class StorageAreaModelConfigurationApplier : IDatabaseConfigurationApplier
{
    public void ApplyConfigurationTo(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StorageAreaModelConfiguration());
    }
}
