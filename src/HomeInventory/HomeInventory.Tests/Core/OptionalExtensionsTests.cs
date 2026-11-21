namespace HomeInventory.Tests.Core;

[UnitTest]
public sealed class OptionalExtensionsTests : BaseTest
{
    private static readonly Guid _value = Guid.NewGuid();
    private readonly Optional<Guid> _some = _value;
    private readonly Optional<Guid> _none = None.Value;

    [Fact]
    public void IsSome_ShouldReturnValue_WhenHasValue()
    {
        var actual = _some.IsSome(out var value);

        using var scope = new AssertionScope();
        actual.Should().BeTrue();
        value.Should().Be(_value);
    }

    [Fact]
    public void IsSome_ShouldReturnFalse_WhenNone()
    {
        var actual = _none.IsSome(out var value);

        using var scope = new AssertionScope();
        actual.Should().BeFalse();
        value.Should().Be(Guid.Empty);
    }

    [Fact]
    public void IsNone_ShouldReturnValue_WhenHasValue()
    {
        var actual = _some.IsNone(out var value);

        using var scope = new AssertionScope();
        actual.Should().BeFalse();
        value.Should().Be(_value);
    }

    [Fact]
    public void IsNone_ShouldReturnTrue_WhenNone()
    {
        var actual = _none.IsNone(out var value);

        using var scope = new AssertionScope();
        actual.Should().BeTrue();
        value.Should().Be(Guid.Empty);
    }

    [Fact]
    public async Task IfAsync_ShouldReturnValue_WhenConditionIsTrue()
    {
        CancellationToken captured = default;

        var actual = await _some.IfAsync((_, t) =>
        {
            captured = t;
            return Task.FromResult(true);
        }, Cancellation.Token);

        using var scope = new AssertionScope();
        actual.Should().Be(_value);
        captured.Should().Be(Cancellation.Token);
    }

    [Fact]
    public async Task IfAsync_ShouldReturnNone_WhenConditionIsFalse()
    {
        var actual = await _some.IfAsync(static (_, _) => Task.FromResult(false), Cancellation.Token);

        actual.Should().BeNone();
    }

    [Fact]
    public async Task IfAsync_ShouldNotInvokeCondition_WhenNone()
    {
        var invoked = false;

        var actual = await _none.IfAsync((_, _) =>
        {
            invoked = true;
            return Task.FromResult(true);
        }, Cancellation.Token);

        using var scope = new AssertionScope();
        actual.Should().BeNone();
        invoked.Should().BeFalse();
    }

    [Fact]
    public async Task SelectAsync_ShouldProject_WhenHasValue()
    {
        CancellationToken captured = default;

        var actual = await Task.FromResult(_some).SelectAsync((x, t) =>
        {
            captured = t;
            return Task.FromResult(x.ToString());
        }, Cancellation.Token);

        using var scope = new AssertionScope();
        actual.Should().Be(_value.ToString());
        captured.Should().Be(Cancellation.Token);
    }

    [Fact]
    public async Task SelectAsync_ShouldNotInvokeSelector_WhenNone()
    {
        var invoked = false;

        var actual = await Task.FromResult(_none).SelectAsync((x, _) =>
        {
            invoked = true;
            return Task.FromResult(x.ToString());
        }, Cancellation.Token);

        using var scope = new AssertionScope();
        actual.Should().BeNone();
        invoked.Should().BeFalse();
    }

    [Fact]
    public void Flatten_ShouldReturnInner_WhenBothHaveValue()
    {
        Optional<Optional<Guid>> nested = _some;

        var actual = nested.Flatten();

        actual.Should().Be(_value);
    }

    [Fact]
    public void Flatten_ShouldReturnNone_WhenInnerIsNone()
    {
        Optional<Optional<Guid>> nested = _none;

        var actual = nested.Flatten();

        actual.Should().BeNone();
    }

    [Fact]
    public void Flatten_ShouldReturnNone_WhenOuterIsNone()
    {
        Optional<Optional<Guid>> nested = None.Value;

        var actual = nested.Flatten();

        actual.Should().BeNone();
    }

    [Fact]
    public void NoneIfNull_ShouldReturnValue_WhenNotNull()
    {
        var expected = new object();

        var actual = expected.NoneIfNull();

        actual.Should().BeSome().Which.Should().BeSameAs(expected);
    }

    [Fact]
    public void NoneIfNull_ShouldReturnNone_WhenNull()
    {
        object? value = null;

        var actual = value.NoneIfNull();

        actual.Should().BeNone();
    }

    [Fact]
    public void ToOptional_ShouldReturnValue_WhenNullableHasValue()
    {
        Guid? nullable = _value;

        var actual = nullable.ToOptional();

        actual.Should().Be(_value);
    }

    [Fact]
    public void ToOptional_ShouldReturnNone_WhenNullableIsNull()
    {
        Guid? nullable = null;

        var actual = nullable.ToOptional();

        actual.Should().BeNone();
    }

    [Fact]
    public void ToOptional_ShouldReturnFirst_WhenEnumerableIsNotEmpty()
    {
        IEnumerable<Guid> enumerable = [_value, Guid.NewGuid()];

        var actual = enumerable.ToOptional();

        actual.Should().Be(_value);
    }

    [Fact]
    public void ToOptional_ShouldReturnNone_WhenEnumerableIsEmpty()
    {
        IEnumerable<Guid> enumerable = [];

        var actual = enumerable.ToOptional();

        actual.Should().BeNone();
    }

    [Fact]
    public void ErrorIfNone_ShouldReturnSuccess_WhenHasValue()
    {
        var actual = _some.ErrorIfNone(Error.New("fail"));

        actual.Should().BeSuccess().Which.Should().Be(_value);
    }

    [Fact]
    public void ErrorIfNone_ShouldReturnError_WhenNone()
    {
        var expected = Error.New("fail");

        var actual = _none.ErrorIfNone(expected);

        actual.Should().BeFail().Which.Should().Be(expected);
    }

    [Fact]
    public void ErrorIfNone_ShouldReturnValidated_WhenHasValidated()
    {
        Validated<Guid, Error> validated = _value;
        Optional<Validated<Guid, Error>> optional = validated;

        var actual = optional.ErrorIfNone(static () => Error.New("fail"));

        actual.Should().BeSuccess().Which.Should().Be(_value);
    }

    [Fact]
    public void ErrorIfNone_ShouldReturnFactoryError_WhenNoValidated()
    {
        var expected = Error.New("fail");
        Optional<Validated<Guid, Error>> optional = None.Value;

        var actual = optional.ErrorIfNone(() => expected);

        actual.Should().BeFail().Which.Should().Be(expected);
    }
}
