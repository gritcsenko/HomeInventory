﻿using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal class ProductAmountModelConfigurationApplier : IDatabaseConfigurationApplier
{
    public void ApplyConfigurationTo(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductAmountModelConfiguration());
    }
}