using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Modules;

public sealed record ModuleBuildContext<TApp>(TApp App) : IModuleBuildContext
    where TApp : IApplicationBuilder, IEndpointRouteBuilder
{
    public IApplicationBuilder ApplicationBuilder => App;

    public IEndpointRouteBuilder EndpointRouteBuilder => App;
    
    internal Task BuildAppAsync(IEnumerable<IModule> modules, CancellationToken cancellationToken = default) => Task.WhenAll(modules.Select(m => m.BuildAppAsync(this, cancellationToken)));
}
