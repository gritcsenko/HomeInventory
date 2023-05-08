using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    private readonly DatabaseContext _context = HomeInventory.Domain.Primitives.TypeExtensions.CreateInstance<DatabaseContext>(
        GetDatabaseOptions(),
        GuidIdFactory.Create(guid => new UserId(guid)))!;

    private readonly FixedTestingDateTimeService _dateTimeService = new() { UtcNow = DateTimeOffset.UtcNow };

    protected private DatabaseContext Context => _context;

    protected IDateTimeService DateTimeService => _dateTimeService;

    private static DbContextOptions<DatabaseContext> GetDatabaseOptions()
        => new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "db").Options;
}
