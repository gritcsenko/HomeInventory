using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly DatabaseContext _context;

    protected BaseDatabaseContextTest()
    {
        _context = DbContextFactory.CreateInMemory<DatabaseContext>(
            DateTime,
            new OutboxDatabaseConfigurationApplier(new PolymorphicDomainEventTypeResolver(new[] { new DomainEventJsonTypeInfo() })),
            new UserModelDatabaseConfigurationApplier());

        AddDisposable(_context);
    }

    protected private DatabaseContext Context => _context;
}
