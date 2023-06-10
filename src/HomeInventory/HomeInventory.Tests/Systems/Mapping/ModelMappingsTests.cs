﻿using HomeInventory.Domain;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class ModelMappingsTests : BaseMappingsTests
{
    public ModelMappingsTests()
    {
        Services.AddDomain();
        Services.AddInfrastructure();
    }

    [Theory]
    [MemberData(nameof(MapData))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Test")]
    public void ShouldMap(object instance, Type destination)
    {
        var sut = CreateSut<ModelMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    [Fact]
    public void ShouldMapUserModelToUser()
    {
        Fixture.CustomizeGuidId<UserId>();
        var sut = CreateSut<ModelMappings>();
        var instance = Fixture.Create<UserModel>();

        var target = sut.Map<User>(instance);

        target.Id.Id.Should().Be(instance.Id.Id);
        target.Email.Value.Should().Be(instance.Email);
        target.Password.Should().Be(instance.Password);
    }

    [Fact]
    public void ShouldProjectUserModelToUser()
    {
        Fixture.CustomizeGuidId<UserId>();
        var sut = CreateSut<ModelMappings>();
        var instance = Fixture.Create<UserModel>();
        var source = new[] { instance }.AsQueryable();

        var target = sut.ProjectTo<User>(source, Cancellation.Token).ToArray();

        target.Should().HaveCount(1);
    }

    [Fact]
    public void ShouldProjectStorageAreaModelToStorageArea()
    {
        Fixture.CustomizeGuidId<StorageAreaId>();
        var sut = CreateSut<ModelMappings>();
        var instance = Fixture.Create<StorageAreaModel>();
        var source = new[] { instance }.AsQueryable();

        var target = sut.ProjectTo<StorageArea>(source, Cancellation.Token).ToArray();

        target.Should().HaveCount(1);
    }

    public static TheoryData<object, Type> MapData()
    {
        var fixture = new Fixture();
        fixture.CustomizeGuidId<UserId>();
        fixture.CustomizeGuidId<ProductId>();
        fixture.CustomizeGuidId<StorageAreaId>();
        fixture.CustomizeEmail();
        fixture.CustomizeFromFactory<string, StorageAreaName>(x => new StorageAreaName(x));

        var items = EnumerationItemsCollection.CreateFor<AmountUnit>();
        fixture.CustomizeFromFactory<int, AmountUnit>(i => items.ElementAt(i % items.Count));
        fixture.CustomizeFromFactory<(decimal value, AmountUnit unit), Amount>(x => new Amount(x.value, x.unit));

        fixture.Customize<ProductAmountModel>(builder =>
            builder.With(m => m.UnitName, (AmountUnit unit) => unit.Name));

        var data = new TheoryData<object, Type>();

        Add<UserId, Guid>(fixture, data);
        Add<StorageAreaId, Guid>(fixture, data);
        Add<ProductId, Guid>(fixture, data);

        Add<Email, string>(fixture, data);
        Add<StorageAreaName, string>(fixture, data);

        Add<User, UserModel>(fixture, data);
        Add<Amount, ProductAmountModel>(fixture, data);
        Add<Product, ProductModel>(fixture, data);
        Add<StorageArea, StorageAreaModel>(fixture, data);

        return data;

        static void Add<T1, T2>(IFixture fixture, TheoryData<object, Type> data)
            where T1 : notnull
            where T2 : notnull
        {
            data.Add(fixture.Create<T1>(), typeof(T2));
            data.Add(fixture.Create<T2>(), typeof(T1));
        }
    }
}
