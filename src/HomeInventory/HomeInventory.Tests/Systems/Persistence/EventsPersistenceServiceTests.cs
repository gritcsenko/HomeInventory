using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class EventsPersistenceServiceTests() : BaseTest<EventsPersistenceServiceTestsGivenContext>(static t => new(t))
{
    private readonly DbContextOptions<DatabaseContext> _options = DbContextFactory.CreateInMemoryOptions<DatabaseContext>("database");

    [Fact]
    public async Task SaveEvents_ShouldPersistDomainEvents()
    {
        Given
            .New(out var dbContextVar, () => DbContextFactory.Default.CreateInMemory(DateTime, _options))
            .Sut(out var sutVar, dbContextVar)
            .New(out var eventsCountVar, () => 3)
            .New<IDomainEvent>(out var domainEventVar, () => new DomainEvent(IdSuppliers.Ulid, DateTime), eventsCountVar)
            .SubstituteFor(out IVariable<IHasDomainEvents> entityVar, e => e.GetDomainEvents().Returns(Given.Variables.GetMany(domainEventVar).AsReadOnly()));

        var then = await When
            .InvokedAsync(sutVar, entityVar, dbContextVar, async (sut, entity, db, t) =>
            {
                await sut.SaveEventsAsync(entity, Cancellation.Token);
                return await db.SaveChangesAsync(t);
            });

        then
            .Result(eventsCountVar, entityVar, (actual, expected, entity) =>
            {
                actual.Should().Be(expected);
                entity.Received().ClearDomainEvents();
            });
    }
}
