using System.Diagnostics.CodeAnalysis;
using HomeInventory.Web.ErrorHandling;
using HomeInventory.Web.ErrorHandling.Interfaces;
using HomeInventory.Web.Framework.Infrastructure;
using HomeInventory.Web.Middleware;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class WebErrorHandlingModuleTests() : BaseModuleTest<WebErrorHandlingModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should()
            .ContainSingleSingleton<ErrorMappingBuilder>()
            .And.ContainSingleSingleton<ErrorMapping>()
            .And.ContainSingleTransient<HomeInventoryProblemDetailsFactory>()
            .And.ContainSingleTransient<ProblemDetailsFactory>()
            .And.ContainSingleTransient<IProblemDetailsFactory>()
            .And.ContainSingleScoped<ICorrelationIdContainer>()
            .And.ContainSingleScoped<CorrelationIdMiddleware>()
            .And.ContainSingleSingleton<IConfigureOptions<JsonOptions>>();
}
