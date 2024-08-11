using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Web.Infrastructure;

public interface IProblemDetailsFactory
{
    ProblemDetails ConvertToProblem(Seq<Error> errors, string? traceIdentifier = null);
}
