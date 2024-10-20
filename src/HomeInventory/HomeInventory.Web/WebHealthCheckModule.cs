﻿using HealthChecks.ApplicationStatus.DependencyInjection;
using HealthChecks.UI.Client;
using HomeInventory.Application;
using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web;

public sealed class WebHealthCheckModule : BaseModule
{
    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
        context.Services
            .AddHealthChecks()
            .AddApplicationStatus();
        context.Services
            .AddHealthChecksUI()
            .AddInMemoryStorage();
    }

    public override async Task BuildAppAsync(ModuleBuildContext context)
    {
        await base.BuildAppAsync(context);

        context.EndpointRouteBuilder
            .MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        context.EndpointRouteBuilder
            .MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = x => x.Tags.Contains(HealthCheckTags.Ready),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        context.EndpointRouteBuilder
            .MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });
        context.ApplicationBuilder
            .UseHealthChecksUI();
    }
}
