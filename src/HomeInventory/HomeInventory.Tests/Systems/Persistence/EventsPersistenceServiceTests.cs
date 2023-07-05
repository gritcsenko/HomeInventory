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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly DatabaseContext _context;

    public EventsPersistenceServiceTests()
    {
        var options = DbContextFactory.CreateInMemoryOptions<DatabaseContext>("database");
        _context = DbContextFactory.CreateInMemory(DateTime, options);

        AddDisposable(_context);
    }

    [Fact]
    public async Task SaveEvents_ShouldPersistDomainEvents()
    {
        _ = Given
            .Add(_dbContext, _context)
            .Sut(_sut, _dbContext.WithIndex(0))
            .Add(_eventsCount, 3)
            .Add(_event, _eventsCount.WithIndex(0), () => new DomainEvent(DateTime))
            .SubstituteFor(_entity,
                (e, v) => e
                .GetDomainEvents()
                .Returns(v.GetMany(_event).ToReadOnly()));

        var then = await When
            .InvokedAsync(_sut.WithIndex(0), _entity.WithIndex(0), _dbContext.WithIndex(0), async (sut, entity, db, t) =>
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

    protected override EventsPersistenceServiceTestsGivenContext CreateGiven(VariablesContainer variables) => new(variables, Fixture);
}

public class EventsPersistenceServiceTestsGivenContext : GivenContext<EventsPersistenceServiceTestsGivenContext>
{

    public EventsPersistenceServiceTestsGivenContext(VariablesContainer variables, IFixture fixture)
        : base(variables, fixture)
    {

    }

    internal EventsPersistenceServiceTestsGivenContext Sut(IVariable<EventsPersistenceService> sutVariable, IIndexedVariable<DatabaseContext> dbContextVariable) =>
        Add(sutVariable, () => new EventsPersistenceService(Variables.Get(dbContextVariable)));
}
