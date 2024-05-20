using DotNext.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace HomeInventory.Web;

internal sealed class ValidationEndpointFilter<T>(IValidator validator, IValidationContextFactory<T> validationContextFactory, IProblemDetailsFactory problemDetailsFactory) : IEndpointFilter
{
    private readonly IValidator _validator = validator;
    private readonly IValidationContextFactory<T> _validationContextFactory = validationContextFactory;
    private readonly IProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arguments = context.Arguments.OfType<T>();
        var httpContext = context.HttpContext;

        var optionalFirstInvalid = await ValidateArgumentAsync(arguments, httpContext.RequestAborted).FirstOrNoneAsync<ValidationResult>(r => !r.IsValid);
        if (optionalFirstInvalid.TryGet(out var result))
        {
            var problem = _problemDetailsFactory.ConvertToProblem(result, httpContext.TraceIdentifier);
            return TypedResults.Problem(problem);
        }

        return await next(context);
    }

    private async IAsyncEnumerable<ValidationResult> ValidateArgumentAsync(IEnumerable<T> arguments, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var argument in arguments)
        {
            var context = _validationContextFactory.CreateContext(argument);
            yield return await _validator.ValidateAsync(context, cancellationToken);
        }
    }
}
