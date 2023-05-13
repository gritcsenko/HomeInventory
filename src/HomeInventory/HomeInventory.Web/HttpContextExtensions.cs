using System.Net;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace HomeInventory.Web;

internal static class HttpContextExtensions
{
    public static IResult Problem(this HttpContext context, ValidationResult result)
    {
        var errors = result.Errors.Select(x => new ValidationError(x.ErrorMessage)).ToArray();
        return context.Problem(errors);
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

    public static IResult Problem(this HttpContext context, IError error)
    {
        return context.Problem(new[] { error });
    }

    private static IResult Problem(this HttpContext context, IReadOnlyCollection<IError> errors)
    {
        var factory = context.GetService<HomeInventoryProblemDetailsFactory>();
        var problem = factory.ConvertToProblem(context, errors, GetStatusCode(errors.First()));
        return Results.Problem(problem);
    }

    private static IResult Problem(this HttpContext context, IReadOnlyCollection<ValidationError> errors)
    {
        var factory = context.GetService<HomeInventoryProblemDetailsFactory>();
        var problem = factory.ConvertToProblem(context, errors, HttpStatusCode.BadRequest);
        return Results.Problem(problem);
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
