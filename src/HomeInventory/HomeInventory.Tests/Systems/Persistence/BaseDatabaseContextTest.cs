using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    private readonly DatabaseContext _context = HomeInventory.Domain.TypeExtensions.CreateInstance<DatabaseContext>(GetDatabaseOptions())!;
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();

    protected private DatabaseContext Context => _context;

    protected IDateTimeService DateTimeService => _dateTimeService;

    private static DbContextOptions<DatabaseContext> GetDatabaseOptions()
        => new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "db").Options;
}
