using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Systems.Persistence;

public class EventsPersistenceServiceTestsGivenContext(BaseTest test) : GivenContext<EventsPersistenceServiceTestsGivenContext>(test)
{
    internal EventsPersistenceServiceTestsGivenContext Sut(out IVariable<EventsPersistenceService> sut, IVariable<DatabaseContext> dbContextVariable) =>
        New(out sut, () => CreateSut(GetValue(dbContextVariable)));

    public EventsPersistenceServiceTestsGivenContext New<T>(out IVariable<T> variable, IVariable<int> countVariable, Func<T> createValue, [CallerArgumentExpression(nameof(variable))] string? name = null)
        where T : notnull =>
        New(out variable, () => Enumerable.Range(0, GetValue(countVariable)).Select(_ => createValue()), name);

    private static EventsPersistenceService CreateSut(DatabaseContext arg) => new(arg);
}
