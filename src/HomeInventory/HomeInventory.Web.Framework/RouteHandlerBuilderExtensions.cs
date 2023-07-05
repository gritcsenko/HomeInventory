using FluentValidation.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithValidationOf<T>(this RouteHandlerBuilder builder) =>
        builder.AddEndpointFilter(new ValidationFilter<T>());

    public static RouteHandlerBuilder WithValidationOf<T>(this RouteHandlerBuilder builder, Action<ValidationStrategy<T>> options) =>
        builder.AddEndpointFilter(new ValidationFilter<T>(options));
}
