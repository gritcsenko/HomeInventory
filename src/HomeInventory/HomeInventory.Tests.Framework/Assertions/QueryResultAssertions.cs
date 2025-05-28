using HomeInventory.Application.Framework.Messaging;

namespace HomeInventory.Tests.Framework.Assertions;

public sealed class QueryResultAssertions<T>(IQueryResult<T> subject, AssertionChain assertionChain) : ReferenceTypeAssertions<IQueryResult<T>, QueryResultAssertions<T>>(subject, assertionChain)
    where T : notnull
{
    protected override string Identifier => "validation";

    public AndWhichConstraint<QueryResultAssertions<T>, Error> BeFail(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Fail{reason}, ", c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsFail)
                .FailWith("but found to be {0}.", Subject));

        return new(this, Subject.Fail);
    }

    public AndWhichConstraint<QueryResultAssertions<T>, T> BeSuccess(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Success{reason}, ", c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsSuccess)
                .FailWith("but found to be {0}.", Subject));

        return new(this, Subject.Success);
    }

    public AndConstraint<QueryResultAssertions<T>> BeSuccess(Action<T> action, string because = "", params object[] becauseArgs)
    {
        BeSuccess(because, becauseArgs);
        Subject.IfSuccess(action);

        return new(this);
    }

    public AndConstraint<QueryResultAssertions<T>> Be(T expected, string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Success {0}{reason}, ", expected, c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsSuccess)
                .ForCondition(subject => subject.Equals(expected))
                .FailWith("but found to be {0}", Subject));

        return new(this);
    }
}
