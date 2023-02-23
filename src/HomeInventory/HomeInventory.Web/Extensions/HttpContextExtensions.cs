using System.Diagnostics;
using System.Net;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Domain.Errors;
using HomeInventory.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace HomeInventory.Web.Extensions;

internal static class HttpContextExtensions
{
    public static IResult Problem(this HttpContext context, ValidationResult result)
    {
        var errors = result.Errors.Select(x => new ValidationError(x.ErrorMessage)).ToArray();
        return context.Problem(errors, HttpStatusCode.BadRequest);
    }

    public static IResult MatchToOk<T, TResponse>(this HttpContext context, OneOf<T, IError> errorOrResult, Func<T, TResponse> onValue) =>
        context.Match(errorOrResult, x => Results.Ok(onValue(x)));

    public static IResult Match<T>(this HttpContext context, OneOf<T, IError> errorOrResult, Func<T, IResult> onValue) =>
        errorOrResult.Match(
            value => onValue(value),
            error => context.Problem(error));

    public static ISender GetSender(this HttpContext context) => context.GetService<ISender>();

    public static IMapper GetMapper(this HttpContext context) => context.GetService<IMapper>();

    public static Task<ValidationResult> ValidateAsync<T>(this HttpContext context, T instance) =>
        context.GetValidator<T>().ValidateAsync(instance, context.RequestAborted);

    public static IValidator<T> GetValidator<T>(this HttpContext context) => context.GetService<IValidator<T>>();

    public static T GetService<T>(this HttpContext context)
        where T : notnull =>
        context.RequestServices.GetRequiredService<T>();

    public static void TryAddTraceId(this ProblemDetails problemDetails, HttpContext context)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }
    }

    public static IResult Problem(this HttpContext context, IError error)
    {
        return context.Problem(new[] { error });
    }

    private static IResult Problem(this HttpContext context, IReadOnlyCollection<IError> errors)
    {
        return context.Problem(errors, GetStatusCode(errors.First()));
    }

    private static IResult Problem(this HttpContext context, IReadOnlyCollection<IError> errors, HttpStatusCode statusCode)
    {
        var problem = ConvertToProblem(errors, statusCode);
        problem.TryAddTraceId(context);

        context.SetItem(HttpContextItems.Errors, errors);

        return Results.Problem(problem);
    }

    private static ProblemDetails ConvertToProblem(IReadOnlyCollection<IError> errors, HttpStatusCode statusCode)
    {
        if (errors.Count == 0)
        {
            throw new InvalidOperationException();
        }
        if (errors.Count == 1)
        {
            return ConvertToProblem(errors.First(), statusCode);
        }
        return new()
        {
            Title = "Multiple Problems",
            Detail = "There were multiple problems that have occurred.",
            Status = (int)statusCode,
            Extensions = {
                ["problems"] = errors.Select(error => error.ConvertToProblem(statusCode)).ToArray()
            },
        };
    }

    public static ProblemDetails ConvertToProblem(this IError error, HttpStatusCode statusCode)
    {
        var result = new ProblemDetails()
        {
            Title = error.GetType().Name,
            Detail = error.Message,
            Status = (int)statusCode,
        };

        foreach (var pair in error.Metadata)
        {
            result.Extensions[pair.Key] = pair.Value;
        }
        return result;
    }

    private static HttpStatusCode GetStatusCode(IError error) =>
        error switch
        {
            ConflictError => HttpStatusCode.Conflict,
            ValidationError => HttpStatusCode.BadRequest,
            NotFoundError => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError,
        };
}
