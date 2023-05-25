using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web;

internal class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().First();
        var validationResult = await context.HttpContext.ValidateAsync(argument);
        if (!validationResult.IsValid)
        {
            return context.HttpContext.Problem(validationResult);
        }

        return await next(context);
    }
}
