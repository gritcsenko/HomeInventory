using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeInventory.Application.Framework.Messaging;

namespace HomeInventory.Web.Framework.Infrastructure;

public static class ProblemDetailsFactoryExtensions
{
    public static ProblemDetails ConvertToProblem(this IProblemDetailsFactory factory, ValidationResult result, string? traceIdentifier = null) =>
        factory.ConvertToProblem(result.Errors, traceIdentifier);

    public static ProblemDetails ConvertToProblem(this IProblemDetailsFactory factory, IEnumerable<ValidationResult> results, string? traceIdentifier = null) =>
        factory.ConvertToProblem(results.SelectMany(static r => r.Errors), traceIdentifier);

    public static Results<Ok<TResponse>, ProblemHttpResult> MatchToOk<T, TResponse>(this IProblemDetailsFactory factory, IQueryResult<T> errorOrResult, Func<T, TResponse> onValue, string? traceIdentifier = null)
        where T : notnull =>
        errorOrResult.Match<Results<Ok<TResponse>, ProblemHttpResult>>(
            value => TypedResults.Ok(onValue(value)),
            error =>
            {
                var problem = factory.ConvertToProblem([error], traceIdentifier);
                return TypedResults.Problem(problem);
            });

    private static ProblemDetails ConvertToProblem(this IProblemDetailsFactory factory, IEnumerable<ValidationFailure> failures, string? traceIdentifier = null) =>
        factory.ConvertToProblem(failures.Select(static x => new ValidationError(x.ErrorMessage, x.AttemptedValue)).ToArray(), traceIdentifier);
}
