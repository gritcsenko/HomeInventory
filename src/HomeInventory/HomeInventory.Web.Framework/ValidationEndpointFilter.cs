using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Web.Framework;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web;

internal sealed class ValidationEndpointFilter<T>(IValidator validator, IValidationContextFactory<T> validationContextFactory) : IEndpointFilter
{
    private readonly IValidator _validator = validator;
    private readonly IValidationContextFactory<T> _validationContextFactory = validationContextFactory;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arguments = context.Arguments.OfType<T>();
        foreach (var argument in arguments)
        {
            var validationResult = await ValidateArgumentAsync(argument);
            if (!validationResult.IsValid)
            {
                return context.HttpContext.Problem(validationResult);
            }
        }

        return await next(context);
    }

    private async Task<ValidationResult> ValidateArgumentAsync(T argument)
    {
        var validationContext = _validationContextFactory.CreateContext(argument);

        return await _validator.ValidateAsync(validationContext);
    }
}
