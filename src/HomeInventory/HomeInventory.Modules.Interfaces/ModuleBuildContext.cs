using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Modules.Interfaces;

public sealed record ModuleBuildContext(IApplicationBuilder ApplicationBuilder, IEndpointRouteBuilder EndpointRouteBuilder)
{
    public T GetRequiredService<T>()
        where T : notnull =>
        ApplicationBuilder.ApplicationServices.GetRequiredService<T>();
}
