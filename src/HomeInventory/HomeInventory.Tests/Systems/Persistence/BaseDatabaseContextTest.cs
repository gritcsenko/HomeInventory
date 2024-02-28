namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly DatabaseContext _context;

    protected BaseDatabaseContextTest()
    {
        _context = DbContextFactory.Default.CreateInMemory<DatabaseContext>(DateTime);

        AddDisposable(_context);
    }

    protected private DatabaseContext Context => _context;
}
