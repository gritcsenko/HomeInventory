namespace HomeInventory.Modules.Interfaces;

public abstract class BaseAttachableModule : BaseModule, IAttachableModule
{
    private IReadOnlyCollection<IModule> _modules = [];

    public virtual void OnAttached(IReadOnlyCollection<IModule> modules) => _modules = modules;

    protected IEnumerable<TModule> FindModules<TModule>()
        where TModule : IModule =>
        _modules.OfType<TModule>();
}
