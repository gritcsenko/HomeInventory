using FluentValidation.Results;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace HomeInventory.Web.Infrastructure;

public static class ProblemDetailsFactoryExtensions
{
    public static ProblemDetails ConvertToProblem(this IProblemDetailsFactory factory, ValidationResult result) =>
        factory.ConvertToProblem(result.Errors.Select(x => new ValidationError(x.ErrorMessage)));

    public static Results<Ok<TResponse>, ProblemHttpResult> MatchToOk<T, TResponse>(this IProblemDetailsFactory factory, OneOf<T, IError> errorOrResult, Func<T, TResponse> onValue) =>
        errorOrResult.Match<Results<Ok<TResponse>, ProblemHttpResult>>(
            value =>
            {
                return TypedResults.Ok(onValue(value));
            },
            error =>
            {
                var problem = factory.ConvertToProblem([error]);
                return TypedResults.Problem(problem);
            });
}
