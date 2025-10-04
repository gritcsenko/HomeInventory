using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Framework.Models.Configuration;

public interface IDatabaseConfigurationApplier
{
    void ApplyConfigurationTo(ModelBuilder modelBuilder);
}
