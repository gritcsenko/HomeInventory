using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    private readonly DatabaseContext _context = HomeInventory.Domain.Primitives.TypeExtensions.CreateInstance<DatabaseContext>(
        GetDatabaseOptions())!;

    protected private DatabaseContext Context => _context;

    private static DbContextOptions<DatabaseContext> GetDatabaseOptions()
        => new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "db").Options;
}
