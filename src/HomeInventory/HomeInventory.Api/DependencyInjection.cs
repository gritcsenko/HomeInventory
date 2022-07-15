using HomeInventory.Api.Common.Errors;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HomeInventory.Tests")]

namespace HomeInventory.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
        services.AddHealthChecks()
            .AddDiskStorageHealthCheck(x => x.CheckAllDrives = true)
            .AddFolder(x => x.AddFolder("C:\\"))
            .AddPrivateMemoryHealthCheck(1024 * 1024 * 1024)
            .AddProcessAllocatedMemoryHealthCheck(1024 * 1024 * 1024)
            .AddProcessHealthCheck("system", _ => true)
            .AddVirtualMemorySizeHealthCheck(1024 * 1024 * 1024)
            .AddWindowsServiceHealthCheck("AppIDSvc", _ => true)
            .AddWorkingSetHealthCheck(1024 * 1024 * 1024);
        services.AddHealthChecksUI(x => x.AddHealthCheckEndpoint("SampleCheck", "/health"))
            .AddInMemoryStorage();

        services.AddSingleton<ProblemDetailsFactory, HomeInventoryProblemDetailsFactory>();

        services.AddMediatR(typeof(DependencyInjection).Assembly);

        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        services.AddScoped<IMapper, ServiceMapper>();

        TypeAdapterConfig.GlobalSettings.Scan(typeof(DependencyInjection).Assembly);

        services.AddControllers(o =>
        {
            o.SuppressAsyncSuffixInActionNames = true;
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
