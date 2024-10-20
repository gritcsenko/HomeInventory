using HomeInventory.Application.Framework.Messaging;
using Execute = FluentAssertions.Execution.Execute;

namespace HomeInventory.Tests.Framework.Assertions;

public sealed class QueryResultAssertions<T>(IQueryResult<T> subject) : ReferenceTypeAssertions<IQueryResult<T>, QueryResultAssertions<T>>(subject)
    where T : notnull
{
    protected override string Identifier => "validation";

    public AndWhichConstraint<QueryResultAssertions<T>, Seq<Error>> BeFail(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Fail{reason}, ")
            .Given(() => Subject)
            .ForCondition(subject => subject.IsFail)
            .FailWith("but found to be {0}.", Subject);

        return new AndWhichConstraint<QueryResultAssertions<T>, Seq<Error>>(this, Subject.FailAsEnumerable());
    }

    public AndWhichConstraint<QueryResultAssertions<T>, T> BeSuccess(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Success{reason}, ")
            .Given(() => Subject)
            .ForCondition(subject => subject.IsSuccess)
            .FailWith("but found to be {0}.", Subject);

        return new AndWhichConstraint<QueryResultAssertions<T>, T>(this, Subject.SuccessAsEnumerable());
    }

    public AndConstraint<QueryResultAssertions<T>> BeSuccess(Action<T> action, string because = "", params object[] becauseArgs)
    {
        BeSuccess(because, becauseArgs);
        Subject.IfSuccess(action);

        return new AndConstraint<QueryResultAssertions<T>>(this);
    }

    public AndConstraint<QueryResultAssertions<T>> Be(T expected, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Success {0}{reason}, ", expected)
            .Given(() => Subject)
            .ForCondition(subject => subject.IsSuccess)
            .ForCondition(subject => subject.Equals(expected))
            .FailWith("but found to be {0}", Subject);

        return new AndConstraint<QueryResultAssertions<T>>(this);
    }
}
