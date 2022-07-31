using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HomeInventory.Tests.Helpers;

internal class ActionResultAssertions : ObjectAssertions<IActionResult, ActionResultAssertions>
{
    public ActionResultAssertions(IActionResult value)
        : base(value)
    {
    }

    public AndConstraint<ActionResultAssertions> HaveStatusCode(int expectedStatusCode)
    {
        BeAssignableTo<IStatusCodeActionResult>()
            .Which.StatusCode.Should().Be(expectedStatusCode);
        return new AndConstraint<ActionResultAssertions>(this);
    }

    public AndConstraint<ActionResultAssertions> NotHaveStatusCode(int expectedStatusCode)
    {
        BeAssignableTo<IStatusCodeActionResult>()
            .Which.StatusCode.Should().NotBe(expectedStatusCode);
        return new AndConstraint<ActionResultAssertions>(this);
    }

    public AndWhichConstraint<ActionResultAssertions, T> HaveValue<T>(T expectedValue)
    {
        BeAssignableTo<ObjectResult>()
            .Which.Value.Should().Be(expectedValue);
        return new AndWhichConstraint<ActionResultAssertions, T>(this, expectedValue);
    }

    public AndWhichConstraint<ActionResultAssertions, T> HaveValueAssignableTo<T>()
    {
        var result = BeAssignableTo<ObjectResult>().Subject;
        result.Value.Should().BeAssignableTo<T>();
        return new AndWhichConstraint<ActionResultAssertions, T>(this, (T)result.Value!);
    }
}
