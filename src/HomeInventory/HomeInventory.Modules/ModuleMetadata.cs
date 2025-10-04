using HomeInventory.Modules.Interfaces;
using LanguageExt;

namespace HomeInventory.Modules;

public sealed class ModuleMetadata(IModule module)
{
    public Type ModuleType { get; } = module.GetType();

    public IModule Module { get; } = module;

    public IEnumerable<Option<ModuleMetadata>> GetDependencies(IReadOnlyCollection<ModuleMetadata> container) =>
        Module.Dependencies.Select(d => container.Where(m => m.ModuleType == d).ToOption());

    public override string ToString() => $"{ModuleType.Name}{(Module.Dependencies.Count == 0 ? "" : ":" + string.Join(',', Module.Dependencies.Select(static d => d.Name)))}";
}
