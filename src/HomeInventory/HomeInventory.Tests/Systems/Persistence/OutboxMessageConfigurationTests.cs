using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class OutboxMessageConfigurationTests : BaseTest
{
    public OutboxMessageConfigurationTests()
    {
        Fixture.CustomizeUlidId<UserId>();
    }

    [Fact]
    public void UserModel_Should_HavePrimaryKey()
    {
        var sut = CreateSut();
        var builder = new ModelBuilder();

        builder.ApplyConfiguration(sut);

        var model = builder.FinalizeModel();
        var type = model.FindRuntimeEntityType(typeof(OutboxMessage));
        type.Should().NotBeNull();
        var primaryKey = type!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(x => x.Name == nameof(OutboxMessage.Id));
    }

    [Fact]
    public void UserModel_Should_HaveContentConfigured()
    {
        var sut = CreateSut();
        var builder = new ModelBuilder();

        builder.ApplyConfiguration(sut);

        var model = builder.FinalizeModel();
        var type = model.FindRuntimeEntityType(typeof(OutboxMessage));
        type.Should().NotBeNull();
        var property = type!.FindProperty(nameof(OutboxMessage.Content));
        property.Should().NotBeNull();
        var converter = property!.GetValueConverter();
        converter.Should().NotBeNull();
        var text = converter!.ConvertToProvider(new UserCreatedDomainEvent(DateTime, Fixture.Create<User>()));
        text.Should().NotBeNull();
    }

    private static OutboxMessageConfiguration CreateSut() => new OutboxDatabaseConfigurationApplier(new PolymorphicDomainEventTypeResolver(new[] { new DomainEventJsonTypeInfo(typeof(UserCreatedDomainEvent)) })).CreateConfiguration();
}
