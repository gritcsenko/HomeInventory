using FluentValidation.Results;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Web.Infrastructure;

public static class ProblemDetailsFactoryExtensions
{
    public static ProblemDetails ConvertToProblem(this IProblemDetailsFactory factory, ValidationResult result, string? traceIdentifier = null) =>
        factory.ConvertToProblem(result.Errors, traceIdentifier);

    public static ProblemDetails ConvertToProblem(this IProblemDetailsFactory factory, IEnumerable<ValidationResult> results, string? traceIdentifier = null) =>
        factory.ConvertToProblem(results.SelectMany(r => r.Errors), traceIdentifier);

    public static Results<Ok<TResponse>, ProblemHttpResult> MatchToOk<T, TResponse>(this IProblemDetailsFactory factory, IQueryResult<T> errorOrResult, Func<T, TResponse> onValue, string? traceIdentifier = null)
        where T : notnull =>
        errorOrResult.Match<Results<Ok<TResponse>, ProblemHttpResult>>(
            value =>
            {
                return TypedResults.Ok(onValue(value));
            },
            error =>
            {
                var problem = factory.ConvertToProblem(error, traceIdentifier);
                return TypedResults.Problem(problem);
            });

    private static ProblemDetails ConvertToProblem(this IProblemDetailsFactory factory, IEnumerable<ValidationFailure> failures, string? traceIdentifier = null) =>
        factory.ConvertToProblem(failures.Select(x => new ValidationError(x.ErrorMessage, x.AttemptedValue)).Cast<Error>().ToSeq(), traceIdentifier);

    public static Results<Ok<TResponse>, ProblemHttpResult> Unwrap<TResponse>(this Results<Ok<Results<Ok<TResponse>, ProblemHttpResult>>, ProblemHttpResult> wrapped)
    {
        if (wrapped.Result is Ok<Results<Ok<TResponse>, ProblemHttpResult>> result)
        {
            return (Ok<TResponse>)result.Value!.Result;
        }

        return (ProblemHttpResult)wrapped.Result;
    }
}
