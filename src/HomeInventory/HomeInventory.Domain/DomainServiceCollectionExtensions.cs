﻿using HomeInventory.Core;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton(IdSuppliers.Cuid);
        services.TryAddSingleton<IScopeAccessor, ScopeAccessor>();
        services.AddSingleton<IAmountFactory, AmountFactory>();
        services.AddTransient<TimeProvider>(_ => new FixedTimeProvider(TimeProvider.System));
        return services;
    }
}
