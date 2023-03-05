using HomeInventory.Web.Extensions;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web;

internal class ValidationFilter<T> : IEndpointFilter
{
    public ValidationFilter()
    {
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arg = context.Arguments.OfType<T>().First();
        var validationResult = await context.HttpContext.ValidateAsync(arg);
        if (!validationResult.IsValid)
        {
            return context.HttpContext.Problem(validationResult);
        }

        return await next(context);
    }
}
