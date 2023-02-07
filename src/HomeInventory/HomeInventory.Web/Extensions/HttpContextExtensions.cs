using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Domain.Errors;
using HomeInventory.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Extensions;

internal static class HttpContextExtensions
{
    public static IResult Problem(this HttpContext context, ValidationResult result)
    {
        var errors = result.Errors.Select(x => new ValidationError(x.ErrorMessage)).ToArray();
        return context.Problem(errors, StatusCodes.Status400BadRequest);
    }

    public static IResult MatchToOk<T, TResponse>(this HttpContext context, Result<T> errorOrResult, Func<T, TResponse> onValue) =>
        context.Match(errorOrResult, x => Results.Ok(onValue(x)));

    public static IResult Match<T>(this HttpContext context, Result<T> errorOrResult, Func<T, IResult> onValue) =>
        errorOrResult.IsSuccess
            ? onValue(errorOrResult.Value)
            : context.Problem(errorOrResult.Errors);

    public static ISender GetSender(this HttpContext context) => context.GetService<ISender>();

    public static IMapper GetMapper(this HttpContext context) => context.GetService<IMapper>();

    public static IValidator<T> GetValidator<T>(this HttpContext context) => context.GetService<IValidator<T>>();

    public static Task<ValidationResult> ValidateAsync<T>(this HttpContext context, T instance, CancellationToken cancellationToken) =>
        context.GetService<IValidator<T>>().ValidateAsync(instance, cancellationToken);

    public static Task<ValidationResult> ValidateAsync<T>(this HttpContext context, T instance) =>
        context.GetService<IValidator<T>>().ValidateAsync(instance, context.RequestAborted);

    public static T GetService<T>(this HttpContext context)
        where T : notnull =>
        context.RequestServices.GetRequiredService<T>();

    private static IResult Problem(this HttpContext context, IReadOnlyCollection<IError> errors, int? statusCode = null)
    {
        context.SetItem(HttpContextItems.Errors, errors);

        var problem = errors.Count > 1
            ? ConvertToProblem(errors)
            : ConvertToProblem(errors.First());

        problem.Status = statusCode ?? GetStatusCode(errors);

        return Results.Problem(problem);
    }

    private static ProblemDetails ConvertToProblem(IReadOnlyCollection<IError> errors) =>
        new()
        {
            Title = "Multiple Problems",
            Detail = "There were multiple problems that have occurred.",
            Extensions = {
                ["problems"] = errors.Select(ConvertToProblem).ToArray()
            },
        };

    private static ProblemDetails ConvertToProblem(IError error) =>
        new()
        {
            Title = error.GetType().Name,
            Detail = error.Message,
        };

    private static int GetStatusCode(IReadOnlyCollection<IError> errors) =>
        errors.First() switch
        {
            ConflictError => StatusCodes.Status409Conflict,
            ValidationError => StatusCodes.Status400BadRequest,
            NotFoundError => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };
}
