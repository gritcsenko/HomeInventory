using HomeInventory.Infrastructure.Persistence;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    protected BaseDatabaseContextTest()
    {
        Context = DbContextFactory.Default.CreateInMemory<DatabaseContext>(DateTime);
        AddAsyncDisposable(Context);
    }

    private protected DatabaseContext Context { get; }
}
