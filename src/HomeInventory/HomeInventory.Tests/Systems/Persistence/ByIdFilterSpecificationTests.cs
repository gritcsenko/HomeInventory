using AutoFixture;
using FastExpressionCompiler;
using FluentAssertions;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class ByIdFilterSpecificationTests : BaseTest
{
    [Fact]
    public void Should_SatisfyWithCorrectId()
    {
        var user = Fixture.Create<UserModel>();
        var id = user.Id;
        var sut = new ByIdFilterSpecification<UserModel>(id);

        var actual = sut.QueryExpression.CompileFast()(user);

        actual.Should().BeTrue();
    }

    [Fact]
    public void Should_NotSatisfyWithWrongId()
    {
        var user = Fixture.Create<UserModel>();
        var id = Fixture.Create<Guid>();
        var sut = new ByIdFilterSpecification<UserModel>(id);

        var actual = sut.QueryExpression.CompileFast()(user);

        actual.Should().BeFalse();
    }
}
