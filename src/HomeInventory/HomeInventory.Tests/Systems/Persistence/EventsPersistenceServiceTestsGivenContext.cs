using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;

namespace HomeInventory.Tests.Systems.Persistence;

public class EventsPersistenceServiceTestsGivenContext(BaseTest test) : GivenContext<EventsPersistenceServiceTestsGivenContext>(test)
{
    internal EventsPersistenceServiceTestsGivenContext Sut(out IVariable<EventsPersistenceService> sutVariable, IVariable<DatabaseContext> dbContextVariable)
    {
        sutVariable = new Variable<EventsPersistenceService>("sut");
        return Add(sutVariable, () => new EventsPersistenceService(GetValue(dbContextVariable)));
    }
}
