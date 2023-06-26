using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly DatabaseContext _context;

    protected BaseDatabaseContextTest()
    {
        var appliers = new IDatabaseConfigurationApplier[]
        {
            new OutboxDatabaseConfigurationApplier(new PolymorphicDomainEventTypeResolver(new[]{ new DomainEventJsonTypeInfo()})),
            new UserModelDatabaseConfigurationApplier(),
        };
        _context = ReflectionMethods.CreateInstance<DatabaseContext>(GetDatabaseOptions(), new PublishDomainEventsInterceptor(Substitute.For<IPublisher>()), DateTime, appliers)!;
        AddDisposable(_context);
    }

    protected private DatabaseContext Context => _context;

    private static DbContextOptions<DatabaseContext> GetDatabaseOptions() =>
        new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: "db" + Ulid.NewUlid())
            .EnableSensitiveDataLogging()
            .Options;
}
