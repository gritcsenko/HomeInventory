using FluentAssertions.Execution;
using Execute = FluentAssertions.Execution.Execute;

namespace HomeInventory.Tests.Framework.Assertions;

public class UlidAssertions(Ulid actualValue) : UlidAssertions<UlidAssertions>(actualValue)
{
}

public class UlidAssertions<TAssertions>(Ulid actualValue)
    where TAssertions : UlidAssertions<TAssertions>
{
    public Ulid Subject { get; } = actualValue;

    public AndConstraint<TAssertions> BeEmpty(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .ForCondition(Subject == Ulid.Empty)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:Ulid} to be empty{reason}, but found {0}.", Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    public AndConstraint<TAssertions> NotBeEmpty(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .ForCondition(Subject != Ulid.Empty)
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect {context:Ulid} to be empty{reason}.");

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    public AndConstraint<TAssertions> Be(string expected, string because = "", params object[] becauseArgs)
    {
        if (!Ulid.TryParse(expected, out var expectedUlid))
        {
            throw new ArgumentException($"Unable to parse \"{expected}\" as a valid ULID", nameof(expected));
        }

        return Be(expectedUlid, because, becauseArgs);
    }

    public AndConstraint<TAssertions> Be(Ulid expected, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .ForCondition(Subject == expected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:Ulid} to be {0}{reason}, but found {1}.", expected, Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    public AndConstraint<TAssertions> NotBe(string unexpected, string because = "", params object[] becauseArgs)
    {
        if (!Ulid.TryParse(unexpected, out var unexpectedGuid))
        {
            throw new ArgumentException($"Unable to parse \"{unexpected}\" as a valid ULID", nameof(unexpected));
        }

        return NotBe(unexpectedGuid, because, becauseArgs);
    }

    public AndConstraint<TAssertions> NotBe(Ulid unexpected, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .ForCondition(Subject != unexpected)
            .BecauseOf(because, becauseArgs)
            .FailWith("Did not expect {context:Ulid} to be {0}{reason}.", Subject);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }
}
