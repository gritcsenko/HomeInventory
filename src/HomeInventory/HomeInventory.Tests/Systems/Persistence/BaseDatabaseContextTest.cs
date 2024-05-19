using HomeInventory.Infrastructure.Persistence;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    private readonly DatabaseContext _context;

    protected BaseDatabaseContextTest()
    {
        _context = DbContextFactory.Default.CreateInMemory<DatabaseContext>(DateTime);
    }

    protected private DatabaseContext Context => _context;

    protected override IEnumerable<IDisposable> InitializeDisposables()
    {
        yield return _context;

        foreach (var disposable in base.InitializeDisposables())
        {
            yield return disposable;
        }
    }
}
