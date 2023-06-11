﻿using AutoMapper;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace HomeInventory.Web;

internal static class HttpContextExtensions
{
    public static Results<Ok<TResponse>, ProblemHttpResult> MatchToOk<T, TResponse>(this HttpContext context, OneOf<T, IError> errorOrResult, Func<T, TResponse> onValue) =>
        errorOrResult.Match<Results<Ok<TResponse>, ProblemHttpResult>>(value => TypedResults.Ok(onValue(value)), error => context.Problem(error));

    public static Task<ValidationResult> ValidateAsync<T>(this HttpContext context, T instance, Action<ValidationStrategy<T>> options) =>
        context.GetValidatorFor<T>().ValidateAsync(instance, options, context.RequestAborted);

    public static IValidator<T> GetValidatorFor<T>(this HttpContext context) =>
        context.GetService<IValidator<T>>();

    public static ISender GetSender(this HttpContext context) =>
        context.GetService<ISender>();

    public static IMapper GetMapper(this HttpContext context) =>
        context.GetService<IMapper>();

    public static T GetService<T>(this HttpContext context)
        where T : notnull =>
        context.RequestServices.GetRequiredService<T>();

    public static ProblemHttpResult Problem(this HttpContext context, ValidationResult result) =>
        context.Problem(result.Errors.Select(x => new ValidationError(x.ErrorMessage)));

    public static ProblemHttpResult Problem(this HttpContext context, params IError[] errors) =>
        context.Problem(errors.AsEnumerable());

    public static TFeature? GetFeature<TFeature>(this HttpContext context) =>
        context.Features.Get<TFeature>();

    private static ProblemHttpResult Problem(this HttpContext context, IEnumerable<IError> errors)
    {
        var factory = context.GetService<HomeInventoryProblemDetailsFactory>();
        var problem = factory.ConvertToProblem(context, errors);
        return TypedResults.Problem(problem);
    }
}
