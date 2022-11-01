using FluentResults;
using FluentValidation.Results;
using HomeInventory.Domain.Errors;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Web.Controllers;

[Authorize]
public abstract class ApiControllerBase : ControllerBase
{
    protected ApiControllerBase()
    {
    }

    protected IActionResult Problem(IReadOnlyCollection<IError> errors)
    {
        HttpContext.SetItem(HttpContextItems.Errors, errors);
        var firstError = errors.First();

        var statusCode = firstError switch
        {
            ConflictError => StatusCodes.Status409Conflict,
            ValidationError => StatusCodes.Status400BadRequest,
            NotFoundError => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };
        return Problem(detail: firstError.Message, statusCode: statusCode, title: firstError.GetType().Name);
    }

    protected IActionResult Problem(ValidationResult result)
    {
        var errors = result.Errors.Select(x => new Error(x.ErrorMessage)).ToArray();

        HttpContext.SetItem(HttpContextItems.Errors, errors);
        var firstError = errors.First();

        var statusCode = firstError switch
        {
            ConflictError => StatusCodes.Status409Conflict,
            ValidationError => StatusCodes.Status400BadRequest,
            NotFoundError => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };
        return Problem(detail: firstError.Message, statusCode: statusCode, title: firstError.GetType().Name);
    }

    protected IActionResult Match<T>(Result<T> errorOrResult, Func<T, IActionResult> onValue) => errorOrResult.IsSuccess ? onValue(errorOrResult.Value) : Problem(errorOrResult.Errors);
}

public class ValidationError : Error
{
    public ValidationError(ValidationResult result)
    {
        var dictionary = result.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => (object)g.Select(x => x.ErrorMessage).ToArray()
            );
        WithMetadata(dictionary);
    }
}
