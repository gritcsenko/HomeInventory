using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web;

internal class ValidationFilter<T> : IEndpointFilter
{
    private readonly Action<ValidationStrategy<T>> _options;

    public ValidationFilter() =>
        _options = _ => { };

    public ValidationFilter(Action<ValidationStrategy<T>> options) =>
        _options = options;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validationResult = await ValidateArgumentAsync(context, 0);
        if (!validationResult.IsValid)
        {
            return context.HttpContext.Problem(validationResult);
        }

        return await next(context);
    }

    private async ValueTask<ValidationResult> ValidateArgumentAsync(EndpointFilterInvocationContext context, int index)
    {
        var argument = context.GetArgument<T>(index);
        var httpContext = context.HttpContext;
        return await httpContext.ValidateAsync(argument, _options);
    }
}
