using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives;
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
    public ModelMappingsTests()
    {
        Services.AddSingleton(GuidIdFactory.Create(id => new UserId(id)));
        Services.AddSingleton<GuidIdConverter<UserId>>();

        Services.AddSingleton(GuidIdFactory.Create(id => new StorageAreaId(id)));
        Services.AddSingleton<GuidIdConverter<StorageAreaId>>();

        Services.AddSingleton<IValueObjectFactory<Email, string>, EmailFactory>();
        Services.AddSingleton<ValueObjectConverter<Email, string>>();
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldMap(object instance, Type destination)
    {
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

            { fixture.Create<StorageAreaId>(), typeof(Guid) },
            { fixture.Create<Guid>(), typeof(StorageAreaId) },

            { fixture.Create<Email>(), typeof(string) },
            { fixture.Create<string>(), typeof(Email) },

            { fixture.Create<User>(), typeof(UserModel) },
            { fixture.Create<UserModel>(), typeof(User) },

            { fixture.Create<Amount>(), typeof(ProductAmountModel) },
            { fixture.Create<ProductAmountModel>(), typeof(Amount) },

            { fixture.Create<Product>(), typeof(ProductModel) },
            { fixture.Create<ProductModel>(), typeof(Product) },

            { fixture.Create<StorageArea>(), typeof(StorageAreaModel) },
            { fixture.Create<StorageAreaModel>(), typeof(StorageArea) },
        };
    }
}
