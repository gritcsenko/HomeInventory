using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Modules;

public sealed class ModuleMetadata(IModule module)
{
    public Type ModuleType { get; } = module.GetType();

    public IModule Module { get; } = module;

    public IEnumerable<Option<ModuleMetadata>> GetDependencies(IReadOnlyCollection<ModuleMetadata> container) =>
        Module.Dependencies.Select(d => container.Find(m => m.ModuleType == d));

    public override string ToString() => $"{ModuleType.Name}{(Module.Dependencies.Count == 0 ? "" : ":" + string.Join(',', Module.Dependencies.Select(d => d.Name)))}";
}