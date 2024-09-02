using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Tests.Modules;

public abstract class BaseModuleTest<TGiven, TModule>(Func<BaseTest, TGiven> createGiven) : BaseTest<TGiven>(createGiven)
    where TGiven : BaseModuleTestGivenContext<TGiven, TModule>
    where TModule : IModule
{
}
