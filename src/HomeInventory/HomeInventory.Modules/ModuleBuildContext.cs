using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Modules;

public sealed record ModuleBuildContext<TApp>(TApp App) : IModuleBuildContext
    where TApp : IApplicationBuilder, IEndpointRouteBuilder
{
    public IApplicationBuilder ApplicationBuilder => App;

    public IEndpointRouteBuilder EndpointRouteBuilder => App;

    public T GetRequiredService<T>()
        where T : notnull =>
        ApplicationBuilder.ApplicationServices.GetRequiredService<T>();
}
