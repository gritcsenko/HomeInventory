using HealthChecks.ApplicationStatus.DependencyInjection;
using HealthChecks.UI.Client;
using HomeInventory.Application;
using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web;

public sealed class WebHealthCheckModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
        context.Services
            .AddHealthChecks()
            .AddApplicationStatus();
        context.Services
            .AddHealthChecksUI()
            .AddInMemoryStorage();
    }

    public override async Task BuildAppAsync(IModuleBuildContext context, CancellationToken cancellationToken = default)
    {
        await base.BuildAppAsync(context: context, cancellationToken: cancellationToken);

        context.EndpointRouteBuilder
            .MapHealthChecks(pattern: "/health", options: new()
            {
                Predicate = static _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });
        context.EndpointRouteBuilder
            .MapHealthChecks(pattern: "/health/ready", options: new()
            {
                Predicate = static x => x.Tags.Contains(item: HealthCheckTags.Ready),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });
        context.EndpointRouteBuilder
            .MapHealthChecks(pattern: "/health/live", options: new()
            {
                Predicate = static _ => false,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });
        context.ApplicationBuilder
            .UseHealthChecksUI();
    }
}
