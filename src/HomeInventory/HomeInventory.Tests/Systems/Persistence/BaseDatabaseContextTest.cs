using HomeInventory.Infrastructure.Persistence;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    private readonly DatabaseContext _context;

    protected BaseDatabaseContextTest()
    {
        AddDisposable(DbContextFactory.Default.CreateInMemory<DatabaseContext>(DateTime), out _context);
    }

    protected private DatabaseContext Context => _context;
}
