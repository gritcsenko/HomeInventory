using AutoMapper;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseRepositoryTest : BaseTest
{
    private readonly IDbContextFactory<DatabaseContext> _factory = Substitute.For<IDbContextFactory<DatabaseContext>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();
    private readonly DatabaseContext _context = HomeInventory.Domain.TypeExtensions.CreateInstance<DatabaseContext>(GetDatabaseOptions())!;

    protected BaseRepositoryTest()
    {
        _factory.CreateDbContextAsync(CancellationToken).Returns(_context);
    }

    protected private IDbContextFactory<DatabaseContext> Factory => _factory;

    protected IMapper Mapper => _mapper;

    protected IDateTimeService DateTimeService => _dateTimeService;

    protected private DatabaseContext Context => _context;

    private static DbContextOptions<DatabaseContext> GetDatabaseOptions()
        => new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "db").Options;
}
