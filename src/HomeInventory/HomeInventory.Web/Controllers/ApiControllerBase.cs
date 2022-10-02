using FluentResults;
using HomeInventory.Domain.Errors;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Web.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    protected IActionResult Match<T>(Result<T> errorOrResult, Func<T, IActionResult> onValue) => errorOrResult.IsSuccess ? onValue(errorOrResult.Value) : Problem(errorOrResult.Errors);
}
