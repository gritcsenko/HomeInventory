using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Web.Framework.Infrastructure;

public interface IProblemDetailsFactory
{
    ProblemDetails ConvertToProblem(Seq<Error> errors, string? traceIdentifier = null);
}
