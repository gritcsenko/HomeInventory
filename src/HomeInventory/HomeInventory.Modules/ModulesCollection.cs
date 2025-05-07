using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Modules;

public class ModulesCollection() : HashSet<IModule>(new ModuleEqualityComparer())
{
    private sealed class ModuleEqualityComparer : IEqualityComparer<IModule>
    {
        public bool Equals(IModule? x, IModule? y) => ReferenceEquals(x, y) || (x?.GetType() == y?.GetType());

        public int GetHashCode(IModule obj) => obj.GetType().GetHashCode();
    }
}