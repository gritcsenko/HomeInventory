using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using Carter;
using FluentResults;
using FluentValidation.Results;
using HomeInventory.Domain.Errors;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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
}
