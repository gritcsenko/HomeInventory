using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeInventory.Tests.Framework.Assertions;

public sealed class OkResultAssertions<TValue>(Ok<TValue> value) : ObjectAssertions<Ok<TValue>, OkResultAssertions<TValue>>(value)
{
    public AndWhichConstraint<OkResultAssertions<TValue>, TValue> HaveValue(TValue expectedValue)
    {
        Subject.Value.Should().Be(expectedValue);
        return new(this, expectedValue);
    }
}
