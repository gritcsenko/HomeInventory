using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Tests.Modules;

[UnitTest]
public abstract class BaseModuleTest<TGiven, TSut>(Func<BaseModuleTest<TGiven, TSut>, TGiven> createGiven) : BaseTest<TGiven>(t => createGiven((BaseModuleTest<TGiven, TSut>)t))
    where TGiven : BaseModuleTestGivenContext<TGiven, TSut>, IModuleTestGivenContext<TGiven>
    where TSut : IModule
{
    [Fact]
    public void ShouldRegisterServices()
    {
        Given
            .Services(out var services)
            .Configuration(out var configuration)
            .FeatureManager(out var featureManager)
            .Sut(out var sut);

        var then = When
            .Invoked(sut, services, configuration, featureManager, (sut, services, configuration, featureManager) => sut.AddServicesAsync(new ModuleServicesContext(services, configuration, featureManager, [])));

        then
            .Ensure(services, services =>
            {
                services.Should().NotBeNullOrEmpty();
                EnsureRegistered(services);
            });
    }

    protected abstract void EnsureRegistered(IServiceCollection services);
}

public abstract class BaseModuleTest<TSut>(Func<TSut> createModuleFunc) : BaseModuleTest<FunctionalModuleTestGivenContext<TSut>, TSut>(t => BaseModuleTestGivenContext.Create(t, createModuleFunc))
    where TSut : IModule
{
}
