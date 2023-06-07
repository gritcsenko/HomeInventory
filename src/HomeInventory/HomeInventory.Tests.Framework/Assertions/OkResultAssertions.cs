using HomeInventory.Tests.Framework.Assertions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeInventory.Tests.Framework.Assertions;

internal class OkResultAssertions<TValue> : ObjectAssertions<Ok<TValue>, OkResultAssertions<TValue>>
{
    public OkResultAssertions(Ok<TValue> value)
        : base(value)
    {
    }

    public AndWhichConstraint<OkResultAssertions<TValue>, TValue> HaveValue(TValue expectedValue)
    {
        Subject.Value.Should().Be(expectedValue);
        return new AndWhichConstraint<OkResultAssertions<TValue>, TValue>(this, expectedValue);
    }
}
