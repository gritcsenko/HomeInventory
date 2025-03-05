using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Modules;

public sealed record ModuleBuildContext<TApp>(TApp App) : IModuleBuildContext
    where TApp : IApplicationBuilder, IEndpointRouteBuilder
{
    public IApplicationBuilder ApplicationBuilder => App;

    public IEndpointRouteBuilder EndpointRouteBuilder => App;
}
