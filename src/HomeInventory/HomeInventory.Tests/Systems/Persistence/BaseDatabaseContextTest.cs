using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
#pragma warning disable CA2213 // Disposable fields should be disposed
    private readonly DatabaseContext _context;
#pragma warning restore CA2213 // Disposable fields should be disposed

    protected BaseDatabaseContextTest()
    {
        _context = ReflectionMethods.CreateInstance<DatabaseContext>(DateTime, GetDatabaseOptions())!;
        AddDisposable(_context);
    }

    protected private DatabaseContext Context => _context;

    private static DbContextOptions<DatabaseContext> GetDatabaseOptions() =>
        new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "db").Options;
}
