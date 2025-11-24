using LanguageExt.Traits;

namespace HomeInventory.Tests.Framework.Assertions;

public class LanguageExtValidationAssertions<TFail, TSuccess>(Validation<TFail, TSuccess> instance, AssertionChain assertionChain) : ReferenceTypeAssertions<Validation<TFail, TSuccess>, LanguageExtValidationAssertions<TFail, TSuccess>>(instance, assertionChain)
    where TFail : Monoid<TFail>
{
    protected override string Identifier => "validation";

    public AndWhichConstraint<LanguageExtValidationAssertions<TFail, TSuccess>, TFail> BeFail(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Fail{reason}, ", c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsFail)
                .FailWith("but found to be {0}.", Subject));

        var failValue = Subject.Match(Fail: f => f, Succ: _ => default!);
        return new(this, failValue);
    }

    public AndWhichConstraint<LanguageExtValidationAssertions<TFail, TSuccess>, TSuccess> BeSuccess(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:validation} to be Success{reason}, ", c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsSuccess)
                .FailWith("but found to be {0}.", Subject));

        var successValue = Subject.Match(Fail: _ => default!, Succ: s => s);
        return new(this, successValue);
    }

    public AndConstraint<LanguageExtValidationAssertions<TFail, TSuccess>> BeSuccess(Action<TSuccess> action, string because = "", params object[] becauseArgs)
    {
        BeSuccess(because, becauseArgs);
        Subject.Success(action).Left(_ => { });

        return new(this);
    }

    public AndConstraint<LanguageExtValidationAssertions<TFail, TSuccess>> Be(TSuccess expected, string because = "", params object[] becauseArgs)
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
