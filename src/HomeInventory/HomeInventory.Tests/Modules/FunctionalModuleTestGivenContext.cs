using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Tests.Modules;

public sealed class FunctionalModuleTestGivenContext<TModule>(BaseTest<FunctionalModuleTestGivenContext<TModule>> test, Func<TModule> createModuleFunc) : BaseModuleTestGivenContext<FunctionalModuleTestGivenContext<TModule>, TModule>(test)
    where TModule : IModule
{
    private readonly Func<TModule> _createModuleFunc = createModuleFunc;

    protected override TModule CreateSut() => _createModuleFunc();
}
