using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Extensions;

internal static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithValidationOf<T>(this RouteHandlerBuilder builder) => builder.AddEndpointFilter(new ValidationFilter<T>());

}
