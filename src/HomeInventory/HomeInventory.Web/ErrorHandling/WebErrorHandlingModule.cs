using HomeInventory.Modules.Interfaces;
using HomeInventory.Web.Configuration.Interfaces;
using HomeInventory.Web.Configuration;
using HomeInventory.Web.Framework.Infrastructure;
using HomeInventory.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.ErrorHandling;

public sealed class WebErrorHandlingModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services
            .AddSingleton(ErrorMappingBuilder.CreateDefault())
            .AddSingleton(static sp => sp.GetRequiredService<ErrorMappingBuilder>().Build())
            .AddTransient<HomeInventoryProblemDetailsFactory>()
            .AddTransient<ProblemDetailsFactory>(static sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>())
            .AddTransient<IProblemDetailsFactory>(static sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>())
            .AddScoped<ICorrelationIdContainer, CorrelationIdContainer>()
            .AddScoped<CorrelationIdMiddleware>()
            .Configure<JsonOptions>(static o => o.SerializerOptions.Converters.Add(new DataContractJsonConverter<Error>()));
    }

    public override async Task BuildAppAsync(IModuleBuildContext context)
    {
        await base.BuildAppAsync(context);

        context.ApplicationBuilder.UseMiddleware<CorrelationIdMiddleware>();
        context.ApplicationBuilder.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandlingPath = "/error", });
        context.EndpointRouteBuilder.Map("/error", static (HttpContext ctx) => Results.Problem(detail: GetFeature<IExceptionHandlerPathFeature>(ctx)?.Error.Message));
    }

    private static TFeature? GetFeature<TFeature>(HttpContext context) =>
      context.Features.Get<TFeature>();
}