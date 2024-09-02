using HomeInventory.Domain;

namespace HomeInventory.Tests.Modules;

public sealed class DomainModuleTestsGivenContext(BaseTest test) : BaseModuleTestGivenContext<DomainModuleTestsGivenContext, DomainModule>(test)
{
    protected override DomainModule CreateSut() => new();
}
