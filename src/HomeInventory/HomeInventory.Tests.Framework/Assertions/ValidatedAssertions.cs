using LanguageExt.Traits;

namespace HomeInventory.Tests.Framework.Assertions;

public class ValidatedAssertions<TFail, TSuccess>(Validated<TSuccess, TFail> instance, AssertionChain assertionChain) : ReferenceTypeAssertions<Validated<TSuccess, TFail>, ValidatedAssertions<TFail, TSuccess>>(instance, assertionChain)
    where TFail : Monoid<TFail>
{
    protected override string Identifier => "validation";

    public AndWhichConstraint<ValidatedAssertions<TFail, TSuccess>, TFail> BeFail(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Fail{reason}, ", c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsFail)
                .FailWith("but found to be {0}.", Subject));

        var failValue = Subject.Match(_ => default!, x => x);
        return new(this, failValue);
    }

    public AndWhichConstraint<ValidatedAssertions<TFail, TSuccess>, TSuccess> BeSuccess(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Success{reason}, ", c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsSuccess)
                .FailWith("but found to be {0}.", Subject));

        var successValue = Subject.Match(x => x, _ => default!);
        return new(this, successValue);
    }

    public AndConstraint<ValidatedAssertions<TFail, TSuccess>> BeSuccess(Action<TSuccess> action, string because = "", params object[] becauseArgs)
    {
        BeSuccess(because, becauseArgs);
        Subject.IfSuccess(action);

        return new(this);
    }

    public AndConstraint<ValidatedAssertions<TFail, TSuccess>> Be(TSuccess expected, string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Success {0}{reason}, ", expected, c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsSuccess)
                .ForCondition(subject => subject == expected)
                .FailWith("but found to be {0}", Subject));

        return new(this);
    }
}
