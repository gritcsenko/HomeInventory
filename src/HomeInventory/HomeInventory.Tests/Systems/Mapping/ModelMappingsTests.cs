using HomeInventory.Api;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class ModelMappingsTests : BaseMappingsTests
{
    private readonly ModulesCollection _modules = [
        new DomainModule(),
        new LoggingModule(),
        new InfrastructureMappingModule(),
    ];

    public ModelMappingsTests()
    {
        var builder = Substitute.For<IHostApplicationBuilder>();
        builder.Services.Returns(Services);
#pragma warning disable CA2000 // Dispose objects before losing scope
        builder.Configuration.Returns(new ConfigurationManager());
#pragma warning restore CA2000 // Dispose objects before losing scope
        _modules.InjectTo(builder);
    }

    [Theory]
    [MemberData(nameof(MapData))]
    public void ShouldMap(object instance, Type destination)
    {
        var sut = CreateSut<ModelMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    public static TheoryData<object, Type> MapData()
    {
        var timestamp = new DateTimeOffset(new DateOnly(2024, 01, 01), TimeOnly.MinValue, TimeSpan.Zero);
        var fixture = new Fixture();
        fixture.CustomizeId<ProductId>(timestamp);

        var items = EnumerationItemsCollection.CreateFor<AmountUnit>();
        fixture.CustomizeFromFactory<AmountUnit, int>(i => items.ElementAt(i % items.Count));
        fixture.CustomizeFromFactory<Amount, (decimal value, AmountUnit unit)>(x => new Amount(x.value, x.unit));

        fixture.Customize<ProductAmountModel>(builder =>
            builder.With(m => m.UnitName, (AmountUnit unit) => unit.Name));

        var data = new TheoryData<object, Type>();

        Add<ProductId, Ulid>(fixture, data);

        Add<Amount, ProductAmountModel>(fixture, data);

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
