using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class EventsPersistenceServiceTests : BaseTest<EventsPersistenceServiceTestsGivenContext>
{
    private readonly DbContextOptions<DatabaseContext> _options;

    public EventsPersistenceServiceTests()
        : base(t => new(t))
    {
        _options = DbContextFactory.CreateInMemoryOptions<DatabaseContext>("database");
    }

    [Fact]
    public async Task SaveEvents_ShouldPersistDomainEvents()
    {
        Given
            .New(out var dbContext, () => DbContextFactory.Default.CreateInMemory(DateTime, _options))
            .Sut(out var sut, dbContext)
            .New(out var eventsCount, () => 3)
            .New<IDomainEvent>(out var domainEvent, eventsCount, () => new DomainEvent(DateTime))
            .SubstituteFor(out IVariable<IHasDomainEvents> entity, e => e.GetDomainEvents().Returns(Given.Variables.GetMany(domainEvent).ToReadOnly()));

        var then = await When
            .InvokedAsync(sut, entity, dbContext, async (sut, entity, db, t) =>
            {
                await sut.SaveEventsAsync(entity, Cancellation.Token);
                return await db.SaveChangesAsync(t);
            });

        then
            .Result(eventsCount, entity, (actual, expected, entity) =>
            {
                actual.Should().Be(expected);
                entity.Received().ClearDomainEvents();
            });
    }
}
