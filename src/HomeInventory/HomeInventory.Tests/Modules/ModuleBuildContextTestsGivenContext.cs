using HomeInventory.Modules;

namespace HomeInventory.Tests.Modules;

public sealed class ModuleBuildContextTestsGivenContext(BaseTest<ModuleBuildContextTestsGivenContext> test) : GivenContext<ModuleBuildContextTestsGivenContext, ModuleBuildContext<SubjectApp>, SubjectApp>(test)
{
    protected override ModuleBuildContext<SubjectApp> CreateSut(SubjectApp arg) => new(arg);
}
