using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Tests.Framework.Assertions;

public class ProblemDetailsAssertions(ProblemDetails value) : ObjectAssertions<ProblemDetails, ProblemDetailsAssertions>(value)
{
}
