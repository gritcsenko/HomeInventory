using HomeInventory.Application;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence.Mapping;

namespace HomeInventory.Tests.Modules;

public class InfrastructureMappingModuleTests() : BaseModuleTest<InfrastructureMappingModule>(() => new())
{
    protected override void EnsureRegistered(IServiceCollection services)
    {
        services.Should().ContainSingleSingleton<AmountObjectConverter>()
            .And.ContainSingleSingleton<IMappingAssemblySource>();
    }
}
