using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Web.Framework.Infrastructure;

public interface IProblemDetailsFactory
{
    ProblemDetails ConvertToProblem(IReadOnlyCollection<Error> errors, string? traceIdentifier = null);
}
