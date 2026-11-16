namespace HomeInventory.Tests.Framework.Assertions;

public class OptionalAssertions<T>(Optional<T> instance, AssertionChain assertionChain) : ReferenceTypeAssertions<Optional<T>, OptionalAssertions<T>>(instance, assertionChain)
{
    protected override string Identifier => "option";

    public AndConstraint<OptionalAssertions<T>> BeNone(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:option} to be None{reason}, ", c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsNone)
                .FailWith("but found to be Some."));

        return new(this);
    }

    public AndWhichConstraint<OptionalAssertions<T>, T> BeSome(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .WithExpectation("Expected {context:option} to be Some{reason}, ", c => c
                .Given(() => Subject)
                .ForCondition(subject => subject.IsSome)
                .FailWith("but found to be None."));

        var value = Subject.Match(onSome: v => v, onNone: () => default!);
        return new(this, value);
    }

    public AndConstraint<OptionalAssertions<T>> BeSome(Action<T> action, string because = "", params object[] becauseArgs)
    {
        BeSome(because, becauseArgs);
        Subject.IfSome(action);

        return new(this);
    }

    public AndConstraint<OptionalAssertions<T>> Be(T expected, string because = "", params object[] becauseArgs)
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
