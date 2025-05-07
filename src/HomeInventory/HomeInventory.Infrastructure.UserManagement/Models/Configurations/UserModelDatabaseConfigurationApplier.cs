using HomeInventory.Infrastructure.Framework.Models.Configuration;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.UserManagement.Models.Configurations;

internal sealed class UserModelDatabaseConfigurationApplier : IDatabaseConfigurationApplier
{
    public void ApplyConfigurationTo(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new UserModelConfiguration());
}
