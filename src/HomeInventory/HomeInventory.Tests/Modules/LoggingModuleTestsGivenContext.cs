using HomeInventory.Api;

namespace HomeInventory.Tests.Modules;

public class LoggingModuleTestsGivenContext(BaseTest test) : BaseModuleTestGivenContext<LoggingModuleTestsGivenContext, LoggingModule>(test)
{
    protected override LoggingModule CreateSut() => new();
}