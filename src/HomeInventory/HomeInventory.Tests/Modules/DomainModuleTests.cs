﻿using HomeInventory.Domain;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Modules;

public sealed class DomainModuleTests() : BaseModuleTest<DomainModule>(() => new())
{
    protected override void EnsureRegistered(IServiceCollection services)
    {
        services.Should().ContainSingleSingleton<IIdSupplier<Ulid>>()
            .And.ContainSingleSingleton<IScopeFactory>()
            .And.ContainSingleSingleton<IScopeContainer>()
            .And.ContainSingleSingleton<IScopeAccessor>()
            .And.ContainSingleSingleton<IAmountFactory>()
            .And.ContainSingleTransient<TimeProvider>();
    }
}
