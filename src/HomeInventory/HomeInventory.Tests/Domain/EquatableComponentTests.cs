﻿using HomeInventory.Domain.Primitives;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class EquatableComponentTests() : BaseTest<EquatableComponentTestsGivenContext>(t => new(t))
{
    [Fact]
    public void GetHashCode_ShoudReturnZero_WhenNoComponents()
    {
        Given
            .Sut(out var sut)
            .EmptyHashCode(out var hash);

        When
            .Invoked(sut, sut => sut.GetHashCode())
            .Result(hash, (actual, hash) => actual.Should().Be(hash.ToHashCode()));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetHashCode_ShoudReturnCombinedComponentsHash_WhenManyComponents(int count)
    {
        Given
            .New<Ulid>(out var component, count)
            .AddAllToHashCode(out var hash, component)
            .Sut(out var sut, component);

        When
            .Invoked(sut, sut => sut.GetHashCode())
            .Result(hash, (actual, hash) => actual.Should().Be(hash.ToHashCode()));
    }

    [Fact]
    public void Equals_ShoudBeEqualToEmpty_WhenNoComponents()
    {
        Given
            .Sut(out var sut, 2);

        When
            .Invoked(sut[0], sut[1], (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeTrue());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Equals_ShoudNotBeEqualToEmpty_WhenManyComponents(int count)
    {
        Given
            .New<Ulid>(out var component, count)
            .Sut(out var sut1, component)
            .Sut(out var sut2);

        When
            .Invoked(sut1, sut2, (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeFalse());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Equals_ShoudBeEqualToComponentWithSameItems_WhenManyComponents(int count)
    {
        Given
            .New<Ulid>(out var component, count)
            .Sut(out var sut, component, 2);

        When
            .Invoked(sut[0], sut[1], (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeTrue());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Equals_ShoudNotBeEqualToComponentWithDifferentItems_WhenManyComponents(int count)
    {
        Given
            .New<Ulid>(out var component, count * 2)
            .Sut(out var sut, component, ..count, count..);

        When
            .Invoked(sut[0], sut[1], (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeFalse());
    }
}

public sealed class EquatableComponentTestsGivenContext : GivenContext<EquatableComponentTestsGivenContext, EquatableComponent<string>, Ulid>
{
    public EquatableComponentTestsGivenContext(BaseTest test)
        : base(test)
    {
        test.Fixture.CustomizeUlid();
    }

    public EquatableComponentTestsGivenContext Sut(out IVariable<EquatableComponent<string>> sut, int count = 1, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        Sut(out sut, new Variable<Ulid>("none"), count, name);

    public new EquatableComponentTestsGivenContext Sut(out IVariable<EquatableComponent<string>> sut, IVariable<Ulid> variable, int count = 1, [CallerArgumentExpression(nameof(sut))] string? name = null) =>
        Sut(out sut, variable, name ?? "sut", Enumerable.Repeat(.., count).ToArray());

    public EquatableComponentTestsGivenContext Sut(out IVariable<EquatableComponent<string>> sut, IVariable<Ulid> variable, params Range[] ranges) =>
        Sut(out sut, variable, "sut", ranges);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3236:Caller information arguments should not be provided explicitly", Justification = "By design")]
    private EquatableComponentTestsGivenContext Sut(out IVariable<EquatableComponent<string>> sut, IVariable<Ulid> variable, string name, params Range[] ranges) =>
        New(out sut, i => CreateSut(variable, ranges[i]), ranges.Length, name);

    private EquatableComponent<string> CreateSut(IVariable<Ulid> variable, Range range) => new(Array.ConvertAll(Variables.GetMany(variable, range).ToArray(), x => (object)x));

    protected override EquatableComponent<string> CreateSut(Ulid arg) => throw new NotImplementedException();
}
