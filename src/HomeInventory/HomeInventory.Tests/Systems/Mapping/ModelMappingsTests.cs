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

        Services.AddSingleton(GuidIdFactory.Create(id => new ProductId(id)));
        Services.AddSingleton<GuidIdConverter<ProductId>>();

        Services.AddSingleton<IValueObjectFactory<Email, string>, EmailFactory>();
        Services.AddSingleton<ValueObjectConverter<Email, string>>();

        Services.AddSingleton<IAmountFactory, AmountFactory>();
        Services.AddSingleton<AmountValueObjectConverter>();
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
        fixture.Customize(GuidIdCustomization.Create(guid => new UserId(guid)));
        fixture.Customize(GuidIdCustomization.Create(guid => new StorageAreaId(guid)));
        fixture.Customize(GuidIdCustomization.Create(guid => new ProductId(guid)));
        fixture.Customize(new EmailCustomization());
        fixture.Customize(new FromFactoryCustomization<int, AmountUnit>(i => AmountUnit.Items.ElementAt(i % AmountUnit.Items.Count)));
        fixture.Customize(new FromFactoryCustomization<(decimal value, AmountUnit unit), Amount>(x => new Amount(x.value, x.unit)));
        fixture.Customize(new FromFactoryCustomization<string, StorageAreaName>(x => new StorageAreaName(x)));

        fixture.Customize<ProductAmountModel>(builder =>
            builder.With(m => m.UnitName, (AmountUnit unit) => unit.Name));

        return new()
        {
            { fixture.Create<UserId>(), typeof(Guid) },
            { fixture.Create<Guid>(), typeof(UserId) },

            { fixture.Create<StorageAreaId>(), typeof(Guid) },
            { fixture.Create<Guid>(), typeof(StorageAreaId) },

            { fixture.Create<ProductId>(), typeof(Guid) },
            { fixture.Create<Guid>(), typeof(ProductId) },

            { fixture.Create<Email>(), typeof(string) },
            { fixture.Create<string>(), typeof(Email) },

            { fixture.Create<StorageAreaName>(), typeof(string) },
            { fixture.Create<string>(), typeof(StorageAreaName) },

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
