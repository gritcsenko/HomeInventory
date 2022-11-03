using AutoFixture;
using FastExpressionCompiler;
using FluentAssertions;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class UserHasEmailSpecificationTests : BaseTest
{
    [Fact]
    public void Should_SatisfyWithCorrectEmail()
    {
        var email = Fixture.Create<string>();
        var user = Fixture.Build<UserModel>()
            .With(x => x.Email, email)
            .Create();
        var sut = new UserHasEmailSpecification(email);

        var actual = sut.QueryExpression.CompileFast()(user);

        actual.Should().BeTrue();
    }

    [Fact]
    public void Should_NotSatisfyWithWrongEmail()
    {
        var user = Fixture.Build<UserModel>()
            .With(x => x.Email, Fixture.Create<string>())
            .Create();
        var sut = new UserHasEmailSpecification(Fixture.Create<string>());

        var actual = sut.QueryExpression.CompileFast()(user);

        actual.Should().BeFalse();
    }
}
