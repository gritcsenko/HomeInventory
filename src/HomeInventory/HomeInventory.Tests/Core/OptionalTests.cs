namespace HomeInventory.Tests.Core;

[UnitTest]
public sealed class OptionalTests
{
    private static readonly Guid _value = Guid.NewGuid();
    private static readonly Guid _other = Guid.NewGuid();
    private readonly Optional<Guid> _some = _value;
    private readonly Optional<Guid> _none = None.Value;

    [Fact]
    public void IsSome_ShouldBeTrue_WhenHasValue()
    {
        using var scope = new AssertionScope();
        _some.IsSome.Should().BeTrue();
        _some.IsNone.Should().BeFalse();
        _some.Count.Should().Be(1);
    }

    [Fact]
    public void IsNone_ShouldBeTrue_WhenNone()
    {
        using var scope = new AssertionScope();
        _none.IsNone.Should().BeTrue();
        _none.IsSome.Should().BeFalse();
        _none.Count.Should().Be(0);
    }

    [Fact]
    public void None_ShouldConvertToNoneOptional()
    {
        Optional<Guid> actual = Optional<Guid>.None;

        actual.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenSameValue()
    {
        Optional<Guid> same = _value;

        using var scope = new AssertionScope();
        _some.Equals(same).Should().BeTrue();
        _some.Equals((object)same).Should().BeTrue();
        (_some == same).Should().BeTrue();
        (_some != same).Should().BeFalse();
        _some.GetHashCode().Should().Be(same.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenDifferentValue()
    {
        Optional<Guid> different = _other;

        using var scope = new AssertionScope();
        _some.Equals(different).Should().BeFalse();
        _some.Equals((object)different).Should().BeFalse();
        (_some == different).Should().BeFalse();
        (_some != different).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparedSomeWithNone()
    {
        using var scope = new AssertionScope();
        _some.Equals(_none).Should().BeFalse();
        _none.Equals(_some).Should().BeFalse();
        (_some == _none).Should().BeFalse();
        (_some != _none).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenBothNone()
    {
        Optional<Guid> otherNone = None.Value;

        using var scope = new AssertionScope();
        _none.Equals(otherNone).Should().BeTrue();
        (_none == otherNone).Should().BeTrue();
        (_none != otherNone).Should().BeFalse();
        _none.GetHashCode().Should().Be(otherNone.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenObjectIsNotOptional()
    {
        using var scope = new AssertionScope();
        _some.Equals(new object()).Should().BeFalse();
        _some.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void OperatorTrue_ShouldBeTrue_WhenHasValue()
    {
        var actual = _some ? "some" : "none";

        actual.Should().Be("some");
    }

    [Fact]
    public void OperatorTrue_ShouldBeFalse_WhenNone()
    {
        var actual = _none ? "some" : "none";

        actual.Should().Be("none");
    }

    [Fact]
    public void GetEnumerator_ShouldYieldValue_WhenHasValue()
    {
        using var scope = new AssertionScope();
        _some.AsEnumerable().Should().Equal(_value);
        new List<Guid>(_some).Should().Equal(_value);
        ((System.Collections.IEnumerable)_some).Cast<Guid>().Should().Equal(_value);
    }

    [Fact]
    public void GetEnumerator_ShouldYieldNothing_WhenNone()
    {
        using var scope = new AssertionScope();
        _none.AsEnumerable().Should().BeEmpty();
        new List<Guid>(_none).Should().BeEmpty();
    }

    [Fact]
    public void Do_ShouldInvokeActionAndReturnSame_WhenHasValue()
    {
        Guid? captured = null;

        var actual = _some.Do(x => captured = x);

        using var scope = new AssertionScope();
        captured.Should().Be(_value);
        actual.Should().Be(_value);
    }

    [Fact]
    public void Do_ShouldNotInvokeAction_WhenNone()
    {
        var invoked = false;

        var actual = _none.Do(_ => invoked = true);

        using var scope = new AssertionScope();
        invoked.Should().BeFalse();
        actual.Should().BeNone();
    }

    [Fact]
    public void Select_ShouldProject_WhenHasValue()
    {
        var actual = _some.Select(static x => x.ToString());

        actual.Should().Be(_value.ToString());
    }

    [Fact]
    public void Select_ShouldReturnNone_WhenNone()
    {
        var actual = _none.Select(static x => x.ToString());

        actual.Should().BeNone();
    }

    [Fact]
    public void SelectMany_ShouldReturnInner_WhenHasValue()
    {
        var actual = _some.SelectMany(static x => (Optional<string>)x.ToString());

        actual.Should().Be(_value.ToString());
    }

    [Fact]
    public void SelectMany_ShouldReturnNone_WhenInnerIsNone()
    {
        var actual = _some.SelectMany(static _ => (Optional<string>)None.Value);

        actual.Should().BeNone();
    }

    [Fact]
    public void SelectMany_ShouldReturnNone_WhenNone()
    {
        var invoked = false;

        var actual = _none.SelectMany(x =>
        {
            invoked = true;
            return (Optional<string>)x.ToString();
        });

        using var scope = new AssertionScope();
        actual.Should().BeNone();
        invoked.Should().BeFalse();
    }

    [Fact]
    public void SelectMany_ShouldCombine_WhenBothHaveValue()
    {
        Optional<Guid> second = _other;

        var actual =
            from a in _some
            from b in second
            select (a, b);

        actual.Should().Be((_value, _other));
    }

    [Fact]
    public void SelectMany_ShouldReturnNone_WhenSecondIsNone()
    {
        var actual =
            from a in _some
            from b in _none
            select (a, b);

        actual.Should().BeNone();
    }

    [Fact]
    public void SelectMany_ShouldReturnNone_WhenFirstIsNone()
    {
        var actual =
            from a in _none
            from b in _some
            select (a, b);

        actual.Should().BeNone();
    }

    [Fact]
    public void IfNone_ShouldReturnValue_WhenHasValue()
    {
        using var scope = new AssertionScope();
        _some.IfNone(_other).Should().Be(_value);
        _some.IfNone(static () => _other).Should().Be(_value);
    }

    [Fact]
    public void IfNone_ShouldReturnFallback_WhenNone()
    {
        using var scope = new AssertionScope();
        _none.IfNone(_other).Should().Be(_other);
        _none.IfNone(static () => _other).Should().Be(_other);
    }

    [Fact]
    public void IfNone_ShouldInvokeAction_OnlyWhenNone()
    {
        var invokedForSome = false;
        var invokedForNone = false;

        _some.IfNone(() => { invokedForSome = true; });
        _none.IfNone(() => { invokedForNone = true; });

        using var scope = new AssertionScope();
        invokedForSome.Should().BeFalse();
        invokedForNone.Should().BeTrue();
    }

    [Fact]
    public void IfSome_ShouldInvokeAction_OnlyWhenHasValue()
    {
        Guid? captured = null;
        var invokedForNone = false;

        _some.IfSome(x => captured = x);
        _none.IfSome(_ => invokedForNone = true);

        using var scope = new AssertionScope();
        captured.Should().Be(_value);
        invokedForNone.Should().BeFalse();
    }

    [Fact]
    public void Match_ShouldUseOnSome_WhenHasValue()
    {
        using var scope = new AssertionScope();
        _some.Match(static x => x, static () => _other).Should().Be(_value);
        _some.Match(static x => x, _other).Should().Be(_value);
    }

    [Fact]
    public void Match_ShouldUseOnNone_WhenNone()
    {
        using var scope = new AssertionScope();
        _none.Match(static x => x, static () => _other).Should().Be(_other);
        _none.Match(static x => x, _other).Should().Be(_other);
    }

    [Fact]
    public void Match_ShouldInvokeOnSomeAction_WhenHasValue()
    {
        Guid? captured = null;
        var noneInvoked = false;

        _some.Match(x => { captured = x; }, () => { noneInvoked = true; });

        using var scope = new AssertionScope();
        captured.Should().Be(_value);
        noneInvoked.Should().BeFalse();
    }

    [Fact]
    public void Match_ShouldInvokeOnNoneAction_WhenNone()
    {
        var someInvoked = false;
        var noneInvoked = false;

        _none.Match(_ => { someInvoked = true; }, () => { noneInvoked = true; });

        using var scope = new AssertionScope();
        someInvoked.Should().BeFalse();
        noneInvoked.Should().BeTrue();
    }

    [Fact]
    public void Coalesce_ShouldReturnFirst_WhenHasValue()
    {
        var invoked = false;

        var actual = _some.Coalesce(() =>
        {
            invoked = true;
            return _other;
        });

        using var scope = new AssertionScope();
        actual.Should().Be(_value);
        invoked.Should().BeFalse();
    }

    [Fact]
    public void Coalesce_ShouldReturnSecond_WhenNone()
    {
        var actual = _none.Coalesce(static () => _other);

        actual.Should().Be(_other);
    }

    [Fact]
    public void Where_ShouldReturnValue_WhenPredicateIsTrue()
    {
        var actual = _some.Where(static _ => true);

        actual.Should().Be(_value);
    }

    [Fact]
    public void Where_ShouldReturnNone_WhenPredicateIsFalse()
    {
        var actual = _some.Where(static _ => false);

        actual.Should().BeNone();
    }

    [Fact]
    public void Where_ShouldReturnNone_WhenNone()
    {
        var actual = _none.Where(static _ => true);

        actual.Should().BeNone();
    }

    [Fact]
    public void ThrowIfNone_ShouldReturnValue_WhenHasValue()
    {
        var actual = _some.ThrowIfNone(static () => new InvalidOperationException());

        actual.Should().Be(_value);
    }

    [Fact]
    public void ThrowIfNone_ShouldThrow_WhenNone()
    {
        var expected = new InvalidOperationException();

        Action action = () => _none.ThrowIfNone(() => expected);

        action.Should().Throw<InvalidOperationException>().Which.Should().BeSameAs(expected);
    }
}
