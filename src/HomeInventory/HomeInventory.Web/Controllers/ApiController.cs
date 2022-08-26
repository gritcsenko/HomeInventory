using ErrorOr;
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

    protected IActionResult Problem(IReadOnlyCollection<Error> errors)
    {
        HttpContext.SetItem(HttpContextItems.Errors, errors);
        var firstError = errors.First();
        var statusCode = firstError.Type switch
        {
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            ErrorType.Unexpected => StatusCodes.Status417ExpectationFailed,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            var x => (int)x,
        };
        return Problem(detail: firstError.Description, statusCode: statusCode, title: firstError.Code);
    }

    protected IActionResult Match<T>(ErrorOr<T> errorOrResult, Func<T, IActionResult> onValue) => errorOrResult.Match(onValue, Problem);
}
