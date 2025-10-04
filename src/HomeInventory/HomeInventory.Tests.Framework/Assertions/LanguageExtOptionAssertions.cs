namespace HomeInventory.Tests.Framework.Assertions;

public class LanguageExtOptionAssertions<T>(Option<T> instance, AssertionChain assertionChain) : ReferenceTypeAssertions<Option<T>, LanguageExtOptionAssertions<T>>(instance, assertionChain)
{
    protected override string Identifier => "option";

    public AndConstraint<LanguageExtOptionAssertions<T>> BeNone(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:option} to be None{reason}, ", c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsNone)
                .FailWith("but found to be Some."));

        return new(this);
    }

    public AndWhichConstraint<LanguageExtOptionAssertions<T>, T> BeSome(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:option} to be Some{reason}, ", c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsSome)
                .FailWith("but found to be None."));

        return new(this, Subject);
    }

    public AndConstraint<LanguageExtOptionAssertions<T>> BeSome(Action<T> action, string because = "", params object[] becauseArgs)
    {
        BeSome(because, becauseArgs);
        Subject.IfSome(action);

        return new(this);
    }

    public AndConstraint<LanguageExtOptionAssertions<T>> Be(T expected, string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:option} to be Some {0}{reason}, ", expected, c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsSome)
                .FailWith("but found to be None.")
                .Then
                .ForCondition(subject => subject.Equals(expected))
                .FailWith("but found Some {0}.", Subject));

        return new(this);
    }
}
