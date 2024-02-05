using System.Net;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Framework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebFramework(this IServiceCollection services)
    {
        services.AddSingleton(_ =>
            new ErrorMappingBuilder()
                .Add<ConflictError>(HttpStatusCode.Conflict)
                .Add<ValidationError>(HttpStatusCode.BadRequest)
                .Add<NotFoundError>(HttpStatusCode.NotFound)
                .Add<InvalidCredentialsError>(HttpStatusCode.Forbidden)
                .SetDefault(HttpStatusCode.InternalServerError)
                .Build());
        services.AddSingleton<HomeInventoryProblemDetailsFactory>();
        services.AddSingleton<ProblemDetailsFactory>(sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>());

        return services;
    }
}
