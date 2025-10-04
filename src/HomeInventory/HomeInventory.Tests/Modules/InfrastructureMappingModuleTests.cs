using System.Diagnostics.CodeAnalysis;
using HomeInventory.Application.Framework;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence.Mapping;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class InfrastructureMappingModuleTests() : BaseModuleTest<InfrastructureMappingModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should().ContainSingleSingleton<AmountObjectConverter>()
            .And.ContainSingleSingleton<IMappingAssemblySource>();
}
