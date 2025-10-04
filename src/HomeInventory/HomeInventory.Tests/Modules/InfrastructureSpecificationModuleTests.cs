using System.Diagnostics.CodeAnalysis;
using Ardalis.Specification;
using HomeInventory.Infrastructure;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class InfrastructureSpecificationModuleTests() : BaseModuleTest<InfrastructureSpecificationModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) => services.Should().ContainSingleSingleton<ISpecificationEvaluator>();
}
