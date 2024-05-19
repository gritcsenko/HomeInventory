using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class EventsPersistenceServiceTests : BaseTest<EventsPersistenceServiceTestsGivenContext>
{
    private static readonly Variable<int> _eventsCount = new(nameof(_eventsCount));
    private static readonly Variable<DatabaseContext> _dbContext = new(nameof(_dbContext));
    private static readonly Variable<EventsPersistenceService> _sut = new(nameof(_sut));
    private static readonly Variable<IHasDomainEvents> _entity = new(nameof(_entity));
    private static readonly Variable<IDomainEvent> _event = new(nameof(_event));

    private readonly DatabaseContext _context;

    public EventsPersistenceServiceTests()
        : base(t => new(t))
    {
        var options = DbContextFactory.CreateInMemoryOptions<DatabaseContext>("database");
        _context = DbContextFactory.Default.CreateInMemory(DateTime, options);
    }

    [Fact]
    public async Task SaveEvents_ShouldPersistDomainEvents()
    {
        _ = Given
            .Add(_dbContext, _context)
            .Sut(_sut, _dbContext)
            .Add(_eventsCount, 3)
            .Add(_event, _eventsCount, () => new DomainEvent(DateTime))
            .SubstituteFor(_entity,
                (e, v) => e
                .GetDomainEvents()
                .Returns(v.GetMany(_event).ToReadOnly()));

        var then = await When
            .InvokedAsync(_sut, _entity, _dbContext, async (sut, entity, db, t) =>
            {
                await sut.SaveEventsAsync(entity, Cancellation.Token);
                return await db.SaveChangesAsync(t);
            });

        then
            .Result(_eventsCount, _entity, (actual, expected, entity) =>
            {
                actual.Should().Be(expected);
                entity.Received().ClearDomainEvents();
            });
    }

    protected override IEnumerable<IDisposable> InitializeDisposables()
    {
        yield return _context;

        foreach (var disposable in base.InitializeDisposables())
        {
            yield return disposable;
        }
    }
}
