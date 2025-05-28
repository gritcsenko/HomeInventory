namespace HomeInventory.Tests.Framework.Assertions;

public class UlidAssertions(Ulid actualValue, AssertionChain assertionChain) : UlidAssertions<UlidAssertions>(actualValue, assertionChain)
{
}

public class UlidAssertions<TAssertions>(Ulid actualValue, AssertionChain assertionChain) : ObjectAssertions<Ulid, TAssertions>(actualValue, assertionChain)
    where TAssertions : UlidAssertions<TAssertions>
{
    public AndConstraint<TAssertions> BeEmpty(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(Subject == Ulid.Empty)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:Ulid} to be empty{reason}, but found {0}.", Subject);

        return new((TAssertions)this);
    }

    public AndConstraint<TAssertions> NotBeEmpty(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .ForCondition(Subject != Ulid.Empty)
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect {context:Ulid} to be empty{reason}.");

        return new((TAssertions)this);
    }

    public AndConstraint<TAssertions> Be(string expected, string because = "", params object[] becauseArgs) =>
        !Ulid.TryParse(expected, out var expectedUlid)
            ? throw new ArgumentException($"Unable to parse \"{expected}\" as a valid ULID", nameof(expected))
            : Be(expectedUlid, because, becauseArgs);

    public AndConstraint<TAssertions> NotBe(string unexpected, string because = "", params object[] becauseArgs) =>
        !Ulid.TryParse(unexpected, out var unexpectedGuid)
            ? throw new ArgumentException($"Unable to parse \"{unexpected}\" as a valid ULID", nameof(unexpected))
            : NotBe(unexpectedGuid, because, becauseArgs);
}
