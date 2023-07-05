using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

public interface IDatabaseConfigurationApplier
{
    void ApplyConfigurationTo(ModelBuilder modelBuilder);
}
