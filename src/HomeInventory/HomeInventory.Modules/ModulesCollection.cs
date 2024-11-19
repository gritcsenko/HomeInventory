using HomeInventory.Modules.Interfaces;
using System.Collections;

namespace HomeInventory.Modules;

public class ModulesCollection : IReadOnlyCollection<IModule>
{
    private readonly System.Collections.Generic.HashSet<IModule> _modules = new(new ModuleEqualityComparer());

    public int Count => _modules.Count;

    public void Add(IModule module) => _modules.Add(module);

    public IEnumerator<IModule> GetEnumerator() => _modules.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private sealed class ModuleEqualityComparer : IEqualityComparer<IModule>
    {
        public bool Equals(IModule? x, IModule? y) => ReferenceEquals(x, y) || (x?.GetType() == y?.GetType());

        public int GetHashCode(IModule obj) => obj.GetType().GetHashCode();
    }
}