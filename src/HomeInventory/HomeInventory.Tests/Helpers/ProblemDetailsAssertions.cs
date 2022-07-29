using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Tests.Helpers;

internal class ProblemDetailsAssertions : ObjectAssertions<ProblemDetails, ProblemDetailsAssertions>
{
    public ProblemDetailsAssertions(ProblemDetails value)
        : base(value)
    {
    }
}
