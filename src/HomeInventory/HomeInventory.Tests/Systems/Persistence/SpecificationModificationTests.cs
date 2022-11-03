using AutoFixture;
using FastExpressionCompiler;
using FluentAssertions;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class SpecificationModificationTests : BaseTest
{
    [Fact]
    public void And_Should_Satisfy_WhenBothAreSatisfied()
    {
        var email = Fixture.Create<string>();
        var user = Fixture.Build<UserModel>()
            .With(x => x.Email, email)
            .Create();
        var id = user.Id;
        var left = new UserHasEmailSpecification(new Email(email));
        var right = new ByIdFilterSpecification<UserModel>(id);
        var sut = left.And(right);

        var actual = sut.QueryExpression.CompileFast()(user);

        actual.Should().BeTrue();
    }

    [Fact]
    public void Or_Should_Satisfy_WhenOneIsSatisfied()
    {
        var email = Fixture.Create<string>();
        var user = Fixture.Build<UserModel>()
            .With(x => x.Email, email)
            .Create();
        var id = Fixture.Create<Guid>();
        var left = new UserHasEmailSpecification(new Email(email));
        var right = new ByIdFilterSpecification<UserModel>(id);
        var sut = left.Or(right);

        var actual = sut.QueryExpression.CompileFast()(user);

        actual.Should().BeTrue();
    }

    [Fact]
    public void Not_Should_SatisfyWithNotSatisfiedOriginal()
    {
        var user = Fixture.Create<UserModel>();
        var id = Fixture.Create<Guid>();
        var sut = new ByIdFilterSpecification<UserModel>(id).Not();

        var actual = sut.QueryExpression.CompileFast()(user);

        actual.Should().BeTrue();
    }
}
