using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;

namespace HomeInventory.Tests.Systems.Persistence;

public class EventsPersistenceServiceTestsGivenContext(BaseTest test) : GivenContext<EventsPersistenceServiceTestsGivenContext>(test)
{
    internal EventsPersistenceServiceTestsGivenContext Sut(IVariable<EventsPersistenceService> sutVariable, IVariable<DatabaseContext> dbContextVariable) =>
        Add(sutVariable, () => new EventsPersistenceService(GetValue(dbContextVariable)));
}
