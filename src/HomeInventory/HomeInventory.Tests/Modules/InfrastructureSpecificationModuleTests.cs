using Ardalis.Specification;

namespace HomeInventory.Tests.Modules;

public class InfrastructureSpecificationModuleTests() : BaseModuleTest<InfrastructureSpecificationModule>(() => new())
{
    protected override void EnsureRegistered(IServiceCollection services)
    {
        services.Should().ContainSingleSingleton<ISpecificationEvaluator>();
    }
}
