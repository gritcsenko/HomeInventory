using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Modules.Interfaces;

public interface IModule
{
    void AddServices(IServiceCollection services, IConfiguration configuration);

    void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder);
}
