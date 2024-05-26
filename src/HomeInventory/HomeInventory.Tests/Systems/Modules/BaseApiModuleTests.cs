using HomeInventory.Domain.ValueObjects;
using Visus.Cuid;

namespace HomeInventory.Tests.Systems.Modules;

public abstract class BaseApiModuleTests<TGiven> : BaseTest<TGiven>
    where TGiven : GivenContext<TGiven>
{
    protected BaseApiModuleTests(Func<BaseTest, TGiven> createGiven)
        : base(createGiven)
    {
        Fixture
            .CustomizeId<UserId>()
            .CustomizeEmail();
    }
}
