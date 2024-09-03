using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public sealed class InfrastructureSpecificationModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton<ISpecificationEvaluator>(SpecificationEvaluator.Default);
    }
}
