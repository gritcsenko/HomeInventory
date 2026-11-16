using HomeInventory.Application.Framework.Messaging;

namespace HomeInventory.Tests.Application;

[UnitTest]
public sealed class QueryResultTests
{
    private static readonly string _value = Guid.NewGuid().ToString();
    private static readonly Error _error = Error.New("fail");
    private readonly IQueryResult<string> _success = QueryResult.From(_value);
    private readonly IQueryResult<string> _fail = QueryResult.From<string>((Validated<string, Error>)_error);

    [Fact]
    public void Success_ShouldReturnValue_WhenSuccess()
    {
        using var scope = new AssertionScope();
        _success.IsSuccess.Should().BeTrue();
        _success.IsFail.Should().BeFalse();
        _success.Success.Should().Be(_value);
        ((IQueryResult)_success).Success.Should().Be(_value);
    }

    [Fact]
    public void Success_ShouldThrow_WhenFail()
    {
        Func<string> action = () => _fail.Success;

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Fail_ShouldReturnError_WhenFail()
    {
        using var scope = new AssertionScope();
        _fail.IsFail.Should().BeTrue();
        _fail.IsSuccess.Should().BeFalse();
        _fail.Fail.Should().BeSameAs(_error);
    }

    [Fact]
    public void Fail_ShouldThrow_WhenSuccess()
    {
        Func<Error> action = () => _success.Fail;

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Equals_ShouldCompareWithSuccessValue()
    {
        using var scope = new AssertionScope();
        _success.Equals(_value).Should().BeTrue();
        _success.Equals(Guid.NewGuid().ToString()).Should().BeFalse();
        _fail.Equals(_value).Should().BeFalse();
    }

    [Fact]
    public void IfSuccess_ShouldInvokeAction_OnlyWhenSuccess()
    {
        string? captured = null;
        var invokedForFail = false;

        _success.IfSuccess(x => captured = x);
        _fail.IfSuccess(_ => invokedForFail = true);

        using var scope = new AssertionScope();
        captured.Should().Be(_value);
        invokedForFail.Should().BeFalse();
    }

    [Fact]
    public void Match_ShouldSelectBranch()
    {
        using var scope = new AssertionScope();
        _success.Match(static x => x, static e => e.Message).Should().Be(_value);
        _fail.Match(static x => x, static e => e.Message).Should().Be(_error.Message);
    }
}
