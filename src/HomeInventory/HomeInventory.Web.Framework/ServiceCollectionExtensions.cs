using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Framework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebFramework(this IServiceCollection services)
    {
        services.AddSingleton<ErrorMapping>();
        services.AddSingleton<HomeInventoryProblemDetailsFactory>();
        services.AddSingleton<ProblemDetailsFactory>(sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>());

        return services;
    }
}
