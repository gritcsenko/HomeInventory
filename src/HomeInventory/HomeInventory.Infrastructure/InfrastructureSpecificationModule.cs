using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeInventory.Infrastructure;

public sealed class InfrastructureSpecificationModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        context.Services.TryAddSingleton<ISpecificationEvaluator>(SpecificationEvaluator.Default);
    }
}
