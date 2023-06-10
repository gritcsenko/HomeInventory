using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Tests.Framework.Assertions;

public class ProblemDetailsAssertions : ObjectAssertions<ProblemDetails, ProblemDetailsAssertions>
{
    public ProblemDetailsAssertions(ProblemDetails value)
        : base(value)
    {
    }
}
