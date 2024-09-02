using HomeInventory.Modules.Interfaces;
using HomeInventory.Web.Configuration.Interfaces;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Framework.Infrastructure;
using HomeInventory.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.ErrorHandling;

public sealed class WebErrorHandling : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(ErrorMappingBuilder.CreateDefault());
        services.AddSingleton(sp => sp.GetRequiredService<ErrorMappingBuilder>().Build());
        services.AddTransient<HomeInventoryProblemDetailsFactory>();
        services.AddTransient<ProblemDetailsFactory>(sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>());
        services.AddTransient<IProblemDetailsFactory>(sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>());

        services.AddScoped<ICorrelationIdContainer, CorrelationIdContainer>();
        services.AddScoped<CorrelationIdMiddleware>();
    }

    public override void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
        applicationBuilder.UseMiddleware<CorrelationIdMiddleware>();
        applicationBuilder.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandlingPath = "/error", });
        endpointRouteBuilder.Map("/error", (HttpContext context) => Results.Problem(detail: GetFeature<IExceptionHandlerPathFeature>(context)?.Error?.Message));
    }

    private static TFeature? GetFeature<TFeature>(HttpContext context) =>
      context.Features.Get<TFeature>();
}
