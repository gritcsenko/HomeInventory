using Carter;
using FluentValidation;
using FluentValidation.Internal;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithValidationOf<T>(this RouteHandlerBuilder builder, Action<ValidationStrategy<T>>? options = null)
    {
        var contextFactory = new ValidationContextFactory<T>(options);
        return builder.AddEndpointFilterFactory((routeHandlerContext, next) =>
        {
            var services = routeHandlerContext.ApplicationServices;
            var problemDetailsFactory = services.GetRequiredService<IProblemDetailsFactory>();
            var filter = new ValidationEndpointFilter<T>(contextFactory, problemDetailsFactory);
            return (context) => filter.InvokeAsync(context, next);
        });
    }

    public static IValidator GetValidator<T>(this IServiceProvider services)
    {
        var locator = services.GetRequiredService<IValidatorLocator>();
        var validator = locator.GetValidator<T>();
        return validator;
    }
}
