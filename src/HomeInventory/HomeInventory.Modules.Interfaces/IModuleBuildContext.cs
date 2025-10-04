using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Modules.Interfaces;

public interface IModuleBuildContext
{
    IApplicationBuilder ApplicationBuilder { get; }

    IEndpointRouteBuilder EndpointRouteBuilder { get; }
}
