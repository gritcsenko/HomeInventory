using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeInventory.Infrastructure;

public sealed class InfrastructureSpecificationModule : BaseModule
{
    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services.TryAddSingleton<ISpecificationEvaluator>(SpecificationEvaluator.Default);
    }
}
