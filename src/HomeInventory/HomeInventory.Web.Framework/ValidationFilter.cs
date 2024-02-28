using Carter;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web;

internal sealed class ValidationFilter<T>(Action<ValidationStrategy<T>> options) : IEndpointFilter
{
    private readonly Action<ValidationStrategy<T>> _options = options;

    public ValidationFilter()
        : this(_ => { })
    {
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validationResult = await ValidateArgumentAsync(context);
        if (!validationResult.IsValid)
        {
            return context.HttpContext.Problem(validationResult);
        }

        return await next(context);
    }

    private async ValueTask<ValidationResult> ValidateArgumentAsync(EndpointFilterInvocationContext context)
    {
        var argument = context.Arguments.OfType<T>().First();
        var validationContext = ValidationContext<T>.CreateWithOptions(argument, _options);

        var httpContext = context.HttpContext;
        var locator = httpContext.GetService<IValidatorLocator>();
        var validator = locator.GetValidator<T>();
        return await validator.ValidateAsync(validationContext);
    }
}
