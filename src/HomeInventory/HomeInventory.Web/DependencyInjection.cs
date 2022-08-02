using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using HomeInventory.Application;
using HomeInventory.Web.Infrastructure;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health
        // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
        services.AddHealthChecks()
            .AddDiskStorageHealthCheck(x => x.CheckAllDrives = true)
            .AddFolder(x => x.AddFolder("C:\\"))
            .AddPrivateMemoryHealthCheck(1024 * 1024 * 1024)
            .AddProcessAllocatedMemoryHealthCheck(1024 * 1024 * 1024)
            .AddProcessHealthCheck("system", _ => true)
            .AddVirtualMemorySizeHealthCheck(3L * 1024 * 1024 * 1024 * 1024)
            .AddWorkingSetHealthCheck(1024 * 1024 * 1024);
        services.AddHealthChecksUI()
            .AddInMemoryStorage();

        services.AddSingleton<ProblemDetailsFactory, HomeInventoryProblemDetailsFactory>();

        services.AddSingleton(sp => new TypeAdapterConfig());
        services.AddScoped<IMapper, ServiceMapper>();
        services.AddMappingSourceFromCurrentAssembly();

        services.AddControllers(o => o.SuppressAsyncSuffixInActionNames = true)
            .AddApplicationPart(Assembly.GetExecutingAssembly())
            .AddControllersAsServices();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddFluentValidation(c =>
        {
            c.DisableDataAnnotationsValidation = true;
        });
        services.AddValidatorsFromAssemblyContaining<Contracts.Validations.IAssemblyMarker>();

        return services;
    }

    public static T UseWeb<T>(this T app)
        where T : IApplicationBuilder, IEndpointRouteBuilder
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = (ctx, report) => ctx.Response.WriteAsJsonAsync(report)
        });
        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = x => x.Tags.Contains("ready"),
            ResponseWriter = (ctx, report) => ctx.Response.WriteAsJsonAsync(report)
        });
        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = (ctx, report) => ctx.Response.WriteAsJsonAsync(report)
        });
        app.UseHealthChecksUI();

        app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandlingPath = "/error", });
        app.Map("/error", (HttpContext context) =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            return Results.Problem(detail: exception?.Message);
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
