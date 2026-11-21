namespace HomeInventory.Tests.Core;

[UnitTest]
public sealed class ValidatedTests
{
    private static readonly Guid _value = Guid.NewGuid();
    private static readonly Error _error = Error.New("fail");
    private readonly Validated<Guid, Error> _success = _value;
    private readonly Validated<Guid, Error> _fail = _error;

    [Fact]
    public void IsSuccess_ShouldBeTrue_WhenSuccess()
    {
        using var scope = new AssertionScope();
        _success.IsSuccess.Should().BeTrue();
        _success.IsFail.Should().BeFalse();
    }

    [Fact]
    public void IsFail_ShouldBeTrue_WhenFail()
    {
        using var scope = new AssertionScope();
        _fail.IsFail.Should().BeTrue();
        _fail.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void IsSuccess_ShouldReturnValue_WhenSuccess()
    {
        var actual = _success.IsSuccess(out var success, out var error);

        using var scope = new AssertionScope();
        actual.Should().BeTrue();
        success.Should().Be(_value);
        error.Should().BeNull();
    }

    [Fact]
    public void IsSuccess_ShouldReturnError_WhenFail()
    {
        var actual = _fail.IsSuccess(out var success, out var error);

        using var scope = new AssertionScope();
        actual.Should().BeFalse();
        success.Should().Be(Guid.Empty);
        error.Should().BeSameAs(_error);
    }

    [Fact]
    public void IsSuccess_ShouldThrow_WhenDefault()
    {
        Validated<Guid, Error> sut = default;

        Action action = () => sut.IsSuccess(out _, out _);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenSameSuccess()
    {
        Validated<Guid, Error> same = _value;

        using var scope = new AssertionScope();
        _success.Equals(same).Should().BeTrue();
        _success.Equals((object)same).Should().BeTrue();
        (_success == same).Should().BeTrue();
        (_success != same).Should().BeFalse();
        _success.GetHashCode().Should().Be(same.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenSameError()
    {
        Validated<Guid, Error> same = _error;

        using var scope = new AssertionScope();
        _fail.Equals(same).Should().BeTrue();
        (_fail == same).Should().BeTrue();
        (_fail != same).Should().BeFalse();
        _fail.GetHashCode().Should().Be(same.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenDifferentSuccess()
    {
        Validated<Guid, Error> different = Guid.NewGuid();

        using var scope = new AssertionScope();
        _success.Equals(different).Should().BeFalse();
        _success.Equals((object)different).Should().BeFalse();
        (_success == different).Should().BeFalse();
        (_success != different).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparedSuccessWithFail()
    {
        using var scope = new AssertionScope();
        _success.Equals(_fail).Should().BeFalse();
        _fail.Equals(_success).Should().BeFalse();
        (_success == _fail).Should().BeFalse();
        (_success != _fail).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenObjectIsNotValidated()
    {
        using var scope = new AssertionScope();
        _success.Equals(new object()).Should().BeFalse();
        _success.Equals((object?)null).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_ShouldReturnZero_WhenDefault()
    {
        Validated<Guid, Error> sut = default;

        sut.GetHashCode().Should().Be(0);
    }

    [Fact]
    public void OperatorTrue_ShouldBeTrue_WhenSuccess()
    {
        var actual = _success ? "success" : "fail";

        actual.Should().Be("success");
    }

    [Fact]
    public void OperatorTrue_ShouldBeFalse_WhenFail()
    {
        var actual = _fail ? "success" : "fail";

        actual.Should().Be("fail");
    }

    [Fact]
    public void Select_ShouldProject_WhenSuccess()
    {
        var actual = _success.Select(static x => x.ToString());

        actual.Should().BeSuccess().Which.Should().Be(_value.ToString());
    }

    [Fact]
    public void Select_ShouldKeepError_WhenFail()
    {
        var invoked = false;

        var actual = _fail.Select(x =>
        {
            invoked = true;
            return x.ToString();
        });

        using var scope = new AssertionScope();
        actual.Should().BeFail().Which.Should().BeSameAs(_error);
        invoked.Should().BeFalse();
    }

    [Fact]
    public void Match_ShouldUseOnSuccess_WhenSuccess()
    {
        var actual = _success.Match(static x => x.ToString(), static e => e.Message);

        actual.Should().Be(_value.ToString());
    }

    [Fact]
    public void Match_ShouldUseOnError_WhenFail()
    {
        var actual = _fail.Match(static x => x.ToString(), static e => e.Message);

        actual.Should().Be(_error.Message);
    }

    [Fact]
    public void ThrowIfFailed_ShouldReturnValue_WhenSuccess()
    {
        var actual = _success.ThrowIfFailed(static _ => new InvalidOperationException());

        actual.Should().Be(_value);
    }

    [Fact]
    public void ThrowIfFailed_ShouldThrow_WhenFail()
    {
        var expected = new InvalidOperationException();
        Error? captured = null;

        Action action = () => _fail.ThrowIfFailed(e =>
        {
            captured = e;
            return expected;
        });

        using var scope = new AssertionScope();
        action.Should().Throw<InvalidOperationException>().Which.Should().BeSameAs(expected);
        captured.Should().BeSameAs(_error);
    }

    [Fact]
    public void ThrowIfSuccess_ShouldReturnError_WhenFail()
    {
        var actual = _fail.ThrowIfSuccess(static _ => new InvalidOperationException());

        actual.Should().BeSameAs(_error);
    }

    [Fact]
    public void ThrowIfSuccess_ShouldThrow_WhenSuccess()
    {
        var expected = new InvalidOperationException();
        Guid? captured = null;

        Action action = () => _success.ThrowIfSuccess(x =>
        {
            captured = x;
            return expected;
        });

        using var scope = new AssertionScope();
        action.Should().Throw<InvalidOperationException>().Which.Should().BeSameAs(expected);
        captured.Should().Be(_value);
    }

    [Fact]
    public void IfSuccess_ShouldInvokeActionAndReturnSame_WhenSuccess()
    {
        Guid? captured = null;

        var actual = _success.IfSuccess(x => captured = x);

        using var scope = new AssertionScope();
        captured.Should().Be(_value);
        actual.Should().BeSuccess().Which.Should().Be(_value);
    }

    [Fact]
    public void IfSuccess_ShouldNotInvokeAction_WhenFail()
    {
        var invoked = false;

        var actual = _fail.IfSuccess(_ => invoked = true);

        using var scope = new AssertionScope();
        invoked.Should().BeFalse();
        actual.Should().BeFail().Which.Should().BeSameAs(_error);
    }

    [Fact]
    public void SuccessOrNone_ShouldReturnValue_WhenSuccess()
    {
        var actual = _success.SuccessOrNone();

        actual.Should().Be(_value);
    }

    [Fact]
    public void SuccessOrNone_ShouldReturnNone_WhenFail()
    {
        var actual = _fail.SuccessOrNone();

        actual.Should().BeNone();
    }

    [Fact]
    public void FailureOrNone_ShouldReturnError_WhenFail()
    {
        var actual = _fail.FailureOrNone();

        actual.Should().BeSome().Which.Should().BeSameAs(_error);
    }

    [Fact]
    public void FailureOrNone_ShouldReturnNone_WhenSuccess()
    {
        var actual = _success.FailureOrNone();

        actual.Should().BeNone();
    }

    [Fact]
    public async Task SelectAsync_ShouldProject_WhenSuccess()
    {
        var actual = await _success.SelectAsync(static x => Task.FromResult(x.ToString()));

        actual.Should().BeSuccess().Which.Should().Be(_value.ToString());
    }

    [Fact]
    public async Task SelectAsync_ShouldKeepError_WhenFail()
    {
        var invoked = false;

        var actual = await _fail.SelectAsync(x =>
        {
            invoked = true;
            return Task.FromResult(x.ToString());
        });

        using var scope = new AssertionScope();
        actual.Should().BeFail().Which.Should().BeSameAs(_error);
        invoked.Should().BeFalse();
    }

    [Fact]
    public async Task MatchAsync_ShouldUseOnSuccess_WhenSuccess()
    {
        var actual = await _success.MatchAsync(static x => Task.FromResult(x.ToString()), static e => e.Message);

        actual.Should().Be(_value.ToString());
    }

    [Fact]
    public async Task MatchAsync_ShouldUseOnError_WhenFail()
    {
        var actual = await _fail.MatchAsync(static x => Task.FromResult(x.ToString()), static e => e.Message);

        actual.Should().Be(_error.Message);
    }
}
