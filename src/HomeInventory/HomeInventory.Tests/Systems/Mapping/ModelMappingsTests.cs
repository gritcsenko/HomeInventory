using AutoFixture.Kernel;
using HomeInventory.Api;
using HomeInventory.Application.Framework;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.Metrics;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class ModelMappingsTests : BaseMappingsTests
{
    private readonly ModulesHost _host = new([new DomainModule(), new LoggingModule(), new InfrastructureMappingModule(), new ApplicationMappingModule()]);
    private readonly IConfiguration _configuration = new ConfigurationManager();
    private readonly IMetricsBuilder _metricsBuilder = Substitute.For<IMetricsBuilder>();

    public ModelMappingsTests()
    {
        Services.AddSingleton(_configuration);

        var timestamp = new DateTimeOffset(new(2024, 01, 01), TimeOnly.MinValue, TimeSpan.Zero);
        Fixture.CustomizeId<ProductId>(timestamp);

        var items = EnumerationItemsCollection.CreateFor<AmountUnit>();
        Fixture.CustomizeFromFactory<AmountUnit, int>(i => items.ElementAt(i % items.Count));
        Fixture.CustomizeFromFactory<Amount, (decimal value, AmountUnit unit)>(x => new(x.value, x.unit));

        Fixture.Customize<ProductAmountModel>(builder => builder.With(m => m.UnitName, (AmountUnit unit) => unit.Name));
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await _host.AddServicesAsync(Services, _configuration, _metricsBuilder);
    }

    [Theory]
    [MemberData(nameof(MapData))]
    public void ShouldMap(Type sourceType, Type destinationType)
    {
        var source = new SpecimenContext(Fixture).Resolve(new SeededRequest(sourceType, null));
        var sut = CreateSut<ModelMappings>();

        var actual = sut.Map(source, sourceType, destinationType);

        actual.Should().BeAssignableTo(destinationType);
    }

    public static TheoryData<Type, Type> MapData()
    {
        var data = new TheoryData<Type, Type>();

        Add<ProductId, Ulid>(data);

        Add<Amount, ProductAmountModel>(data);

        return data;

        static void Add<T1, T2>(TheoryData<Type, Type> data)
            where T1 : notnull
            where T2 : notnull
        {
            data.Add(typeof(T1), typeof(T2));
            data.Add(typeof(T2), typeof(T1));
        }
    }
}
