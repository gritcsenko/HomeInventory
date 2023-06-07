using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Tests.Framework.Assertions;

internal class ProblemDetailsAssertions : ObjectAssertions<ProblemDetails, ProblemDetailsAssertions>
{
    public ProblemDetailsAssertions(ProblemDetails value)
        : base(value)
    {
    }
}
