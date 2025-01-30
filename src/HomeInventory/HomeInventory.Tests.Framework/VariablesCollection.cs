namespace HomeInventory.Tests.Framework;

public sealed class VariablesContainer
{
    private readonly Dictionary<string, IVariableValues> _variables = [];

    public void Add<T>(IVariable<T> variable, Func<T> createValueFunc) =>
        AllFor(variable).Add(createValueFunc);

    public async Task AddAsync<T>(IVariable<T> variable, Func<Task<T>> createValueFunc) => 
        await AllFor(variable).AddAsync(createValueFunc);

    public Option<PropertyValue<T>> TryGet<T>(IIndexedVariable<T> variable) =>
        AllFor(variable).TryGet(variable.Index);

    public Option<PropertyValue<T>> TryGetOrAdd<T>(IIndexedVariable<T> variable, Func<T> createValueFunc) =>
        AllFor(variable).TryGetOrAdd(variable.Index, createValueFunc);

    public IEnumerable<T> GetAll<T>(IVariable<T> variable) =>
        AllFor(variable).Values();

    public Option<PropertyValue<T>> TryUpdate<T>(IIndexedVariable<T> variable, Func<T> createValueFunc) =>
        AllFor(variable).TrySet(variable.Index, createValueFunc);

    private VariableValues<T> AllFor<T>(IVariable<T> variable) =>
        _variables.GetOrAdd(variable.Name, static _ => new VariableValues<T>());
}
