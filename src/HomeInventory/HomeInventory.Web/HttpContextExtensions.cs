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
        var errors = result.Errors.Select(x => new ValidationError(x.ErrorMessage));
        return context.Problem(errors);
    }

    public static IResult MatchToOk<T, TResponse>(this HttpContext context, OneOf<T, IError> errorOrResult, Func<T, TResponse> onValue) =>
        context.Match(errorOrResult, value => Results.Ok(onValue(value)));

    public static IResult Match<T>(this HttpContext context, OneOf<T, IError> errorOrResult, Func<T, IResult> onValue) =>
        errorOrResult.Match(value => onValue(value), error => context.Problem(error));

    public static Task<ValidationResult> ValidateAsync<T>(this HttpContext context, T instance) =>
        context.GetService<IValidator<T>>().ValidateAsync(instance, context.RequestAborted);

    public static ISender GetSender(this HttpContext context) =>
        context.GetService<ISender>();

    public static IMapper GetMapper(this HttpContext context) =>
        context.GetService<IMapper>();

    public static T GetService<T>(this HttpContext context)
        where T : notnull =>
        context.RequestServices.GetRequiredService<T>();

    public static IResult Problem(this HttpContext context, params IError[] errors) =>
        context.Problem(errors.AsEnumerable());

    private static IResult Problem(this HttpContext context, IEnumerable<IError> errors)
    {
        var factory = context.GetService<HomeInventoryProblemDetailsFactory>();
        var problem = factory.ConvertToProblem(context, errors);
        return Results.Problem(problem);
    }
}
