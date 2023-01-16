using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using AutoMapper;
using Carter;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Domain.Errors;
using HomeInventory.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web;

internal abstract class ApiModule : CarterModule
{
    protected ApiModule()
    {
    }

    protected static ApiVersionSet GetVersionSet(IEndpointRouteBuilder app) =>
        app.NewApiVersionSet()
            .HasApiVersion(1)
            .ReportApiVersions()
            .Build();

    protected static IResult Problem(HttpContext context, IReadOnlyCollection<IError> errors)
    {
        context.SetItem(HttpContextItems.Errors, errors);
        var firstError = errors.First();

        var statusCode = firstError switch
        {
            ConflictError => StatusCodes.Status409Conflict,
            ValidationError => StatusCodes.Status400BadRequest,
            NotFoundError => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };
        return Results.Problem(detail: firstError.Message, statusCode: statusCode, title: firstError.GetType().Name);
    }

    protected static IResult Problem(HttpContext context, ValidationResult result)
    {
        var errors = result.Errors.Select(x => new Error(x.ErrorMessage)).ToArray();
        return Problem(context, errors);
    }

    protected static IResult Match<T>(HttpContext context, Result<T> errorOrResult, Func<T, IResult> onValue) => errorOrResult.IsSuccess
        ? onValue(errorOrResult.Value)
        : Problem(context, errorOrResult.Errors);

    protected static ISender GetSender(HttpContext context) => GetService<ISender>(context);

    protected static IMapper GetMapper(HttpContext context) => GetService<IMapper>(context);

    protected static IValidator<T> GetValidator<T>(HttpContext context) => GetService<IValidator<T>>(context);

    protected static Task<ValidationResult> ValidateAsync<T>(HttpContext context, T instance, CancellationToken cancellationToken) =>
        GetService<IValidator<T>>(context).ValidateAsync(instance, cancellationToken);

    protected static T GetService<T>(HttpContext context)
        where T : notnull
        => context.RequestServices.GetRequiredService<T>();
}
