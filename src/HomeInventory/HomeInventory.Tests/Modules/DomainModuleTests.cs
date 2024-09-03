using HomeInventory.Domain;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Modules;

public sealed class DomainModuleTests() : BaseModuleTest<DomainModuleTestsGivenContext>(t => new(t))
{
    [Fact]
    public void ShouldRegisterServices()
    {
        Given
            .Services(out var services)
            .Configuration(out var configuration)
            .Sut(out var sut);

        var then = When
            .Invoked(sut, services, configuration, (sut, services, configuration) => sut.AddServices(services, configuration));

        then
            .Ensure(services, services =>
            {
                services.Should().ContainSingleSingleton<IIdSupplier<Ulid>>()
                    .And.ContainSingleSingleton<IScopeFactory>()
                    .And.ContainSingleSingleton<IScopeContainer>()
                    .And.ContainSingleSingleton<IScopeAccessor>()
                    .And.ContainSingleSingleton<IAmountFactory>()
                    .And.ContainSingleTransient<TimeProvider>();
            });
    }
}
