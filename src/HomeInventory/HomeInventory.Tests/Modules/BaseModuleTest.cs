using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Tests.Modules;

[UnitTest]
public abstract class BaseModuleTest<TGiven>(Func<BaseTest, TGiven> createGiven) : BaseTest<TGiven>(createGiven)
    where TGiven : GivenContext<TGiven>, IModuleTestGivenContext<TGiven>
{
}
