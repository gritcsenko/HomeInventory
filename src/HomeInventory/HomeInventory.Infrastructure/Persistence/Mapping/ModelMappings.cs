﻿using HomeInventory.Application.Mapping;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal sealed class ModelMappings : MappingProfile
{
    public ModelMappings()
    {
        CreateMapForId<ProductId>();

        CreateMapForValue<Amount, ProductAmountModel, AmountObjectConverter>(obj => new ProductAmountModel { Value = obj.Value, UnitName = obj.Unit.Name });
    }
}
