using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Web.Infrastructure;

public interface IProblemDetailsFactory
{
    ProblemDetails ConvertToProblem(IEnumerable<IError> errors);
}
