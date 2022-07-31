using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Entities;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Systems.Mapping;

[Trait("Category", "Unit")]
public class QueryMappingsTests : BaseMappingsTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldMap(object instance, Type destination)
    {
        var sut = CreateSut<QueryMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    public static TheoryData<object, Type> Data()
    {
        var fixture = new Fixture();
        return new()
        {
            { fixture.Create<AuthenticateQuery>(), typeof(FilterSpecification<User>) },
        };
    }
}
