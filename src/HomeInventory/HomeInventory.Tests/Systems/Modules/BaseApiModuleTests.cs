using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Modules;

public abstract class BaseApiModuleTests<TGiven> : BaseTest<TGiven>
    where TGiven : GivenContext<TGiven>
{
    protected BaseApiModuleTests(Func<BaseTest, TGiven> createGiven)
        : base(createGiven)
    {
        Fixture
            .CustomizeUlidId<UserId>()
            .CustomizeEmail()
            .CustomizeFromFactory<Ulid, ISupplier<Ulid>>(_ => new ValueSupplier<Ulid>(Ulid.NewUlid()));
    }
}
