using System.Diagnostics.CodeAnalysis;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public sealed class DomainModuleTests() : BaseModuleTest<DomainModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should().ContainSingleSingleton<IIdSupplier<Ulid>>()
            .And.ContainSingleSingleton<IScopeFactory>()
            .And.ContainSingleSingleton<IScopeContainer>()
            .And.ContainSingleSingleton<IScopeAccessor>()
            .And.ContainSingleSingleton<IAmountFactory>()
            .And.ContainSingleTransient<TimeProvider>();
}
