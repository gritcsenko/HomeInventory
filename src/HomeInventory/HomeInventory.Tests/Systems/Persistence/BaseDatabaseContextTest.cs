using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly DatabaseContext _context;

    protected BaseDatabaseContextTest()
    {
        _context = ReflectionMethods.CreateInstance<DatabaseContext>(GetDatabaseOptions(), new PublishDomainEventsInterceptor(Substitute.For<IPublisher>()))!;
        AddDisposable(_context);
    }

    protected private DatabaseContext Context => _context;

    private static DbContextOptions<DatabaseContext> GetDatabaseOptions() =>
        new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "db").Options;
}
