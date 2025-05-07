using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Modules;

public interface IRegisteredModules
{
    Task BuildApplicationAsync<TApp>(TApp app, CancellationToken cancellationToken = default)
        where TApp : IApplicationBuilder, IEndpointRouteBuilder;
}
