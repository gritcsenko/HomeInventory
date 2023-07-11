using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal class ProductModelConfigurationApplier : IDatabaseConfigurationApplier
{
    public void ApplyConfigurationTo(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductModelConfiguration());
    }
}
