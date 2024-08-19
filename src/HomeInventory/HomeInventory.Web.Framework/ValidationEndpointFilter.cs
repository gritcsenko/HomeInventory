using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace HomeInventory.Web;

internal sealed class ValidationEndpointFilter<T>(IValidationContextFactory<T> validationContextFactory, IProblemDetailsFactory problemDetailsFactory) : IEndpointFilter
{
    private readonly IValidationContextFactory<T> _validationContextFactory = validationContextFactory;
    private readonly IProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arguments = context.Arguments.OfType<T>();
        var httpContext = context.HttpContext;
        var validator = httpContext.RequestServices.GetValidator<T>();

        var results = await ValidateArgumentAsync(validator, arguments, httpContext.RequestAborted).ToArrayAsync(httpContext.RequestAborted);
        if (results.Length == 0 || Array.TrueForAll(results, r => r.IsValid))
        {
            return await next(context);
        }

        var problem = _problemDetailsFactory.ConvertToProblem(results.Where(r => !r.IsValid), httpContext.TraceIdentifier);
        return TypedResults.Problem(problem);
    }

    private async IAsyncEnumerable<ValidationResult> ValidateArgumentAsync(IValidator validator, IEnumerable<T> arguments, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var argument in arguments.WithCancellation(cancellationToken))
        {
            var context = _validationContextFactory.CreateContext(argument);
            yield return await validator.ValidateAsync(context, cancellationToken);
        }
    }
}
