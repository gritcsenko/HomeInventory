using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Systems.Persistence;

public class EventsPersistenceServiceTestsGivenContext(BaseTest test) : GivenContext<EventsPersistenceServiceTestsGivenContext>(test)
{
    internal EventsPersistenceServiceTestsGivenContext Sut(out IVariable<EventsPersistenceService> sutVariable, IVariable<DatabaseContext> dbContextVariable)
    {
        sutVariable = new Variable<EventsPersistenceService>("sut");
        return Add(sutVariable, () => new EventsPersistenceService(GetValue(dbContextVariable)));
    }

    public EventsPersistenceServiceTestsGivenContext New<T>(out IVariable<T> variable, IVariable<int> countVariable, Func<T> createValue, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull
    {
        variable = new Variable<T>(name ?? typeof(T).Name);
        foreach (var _ in Enumerable.Range(0, GetValue(countVariable)))
        {
            Add(variable, createValue);
        }

        return This;
    }
}
