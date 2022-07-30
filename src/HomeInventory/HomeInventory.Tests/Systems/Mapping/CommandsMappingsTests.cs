using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Application.Mapping;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Systems.Mapping;

[Trait("Category", "Unit")]
public class CommandsMappingsTests : BaseMappingsTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldMap(object instance, Type destination)
    {
        var sut = CreateSut<CommandsMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    public static TheoryData<object, Type> Data()
    {
        var fixture = new Fixture();
        return new()
        {
            { fixture.Create<RegisterCommand>(), typeof(UserHasEmailSpecification) },
            { fixture.Create<RegisterCommand>(), typeof(CreateUserSpecification) },
        };
    }
}
