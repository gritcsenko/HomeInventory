using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace HomeInventory.Web.Framework;

internal sealed class ValidationEndpointFilter<TArg>(IValidationContextFactory<TArg> validationContextFactory, IProblemDetailsFactory problemDetailsFactory) : IEndpointFilter
{
    private readonly IValidationContextFactory<TArg> _validationContextFactory = validationContextFactory;
    private readonly IProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.Count == 0)
        {
            return await next(context);
        }

        var arguments = context.Arguments.OfType<TArg>();
        var httpContext = context.HttpContext;
        var validator = httpContext.RequestServices.GetValidator<TArg>();

        var results = await ValidateArgumentAsync(validator, arguments, httpContext.RequestAborted).ToArrayAsync(httpContext.RequestAborted);
        if (Array.TrueForAll(results, static r => r.IsValid))
        {
            return await next(context);
        }

        var problem = _problemDetailsFactory.ConvertToProblem(results.Where(static r => !r.IsValid), httpContext.TraceIdentifier);
        return TypedResults.Problem(problem);
    }

    private async IAsyncEnumerable<ValidationResult> ValidateArgumentAsync(IValidator validator, IEnumerable<TArg> arguments, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var argument in arguments)
        {
            var context = _validationContextFactory.CreateContext(argument);
            yield return await validator.ValidateAsync(context, cancellationToken);
        }
    }
}
