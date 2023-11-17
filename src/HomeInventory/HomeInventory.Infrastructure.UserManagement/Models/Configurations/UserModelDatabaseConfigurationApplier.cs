﻿using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal class UserModelDatabaseConfigurationApplier : IDatabaseConfigurationApplier
{
    public void ApplyConfigurationTo(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new UserModelConfiguration());
}
