using AutoFixture;
using FluentAssertions;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Systems.Mapping;

[Trait("Category", "Unit")]
public class ModelMappingsTests : BaseMappingsTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldMap(object instance, Type destination)
    {
        Services.AddSingleton(GuidIdFactory.Create(id => new UserId(id)));
        Services.AddSingleton<IEmailFactory, EmailFactory>();
        Services.AddSingleton<UserIdConverter>();
        Services.AddSingleton<EmailConverter>();

        var sut = CreateSut<ModelMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    public static TheoryData<object, Type> Data()
    {
        var fixture = new Fixture();
        fixture.Customize(new UserIdCustomization());
        fixture.Customize(new EmailCustomization());
        return new()
        {
            { fixture.Create<UserId>(), typeof(Guid) },
            { fixture.Create<Guid>(), typeof(UserId) },
            { fixture.Create<User>(), typeof(UserModel) },
            { fixture.Create<UserModel>(), typeof(User) },
        };
    }
}
