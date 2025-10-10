using HomeInventory.Infrastructure.Framework;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseRepositoryTest : BaseDatabaseContextTest
{
    protected IEventsPersistenceService PersistenceService { get; } = Substitute.For<IEventsPersistenceService>();

    protected TimeProvider TimeProvider { get; } = new FixedTimeProvider(TimeProvider.System);
}
