namespace HomeInventory.Modules.Interfaces;

public interface IAttachableModule : IModule
{
    void OnAttached(IReadOnlyCollection<IModule> modules);
}
