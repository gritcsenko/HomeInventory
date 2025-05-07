using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Tests.Modules;

public sealed class SubjectModule : BaseModule
{
    public new void DependsOn<TModule>()
        where TModule : class, IModule =>
        base.DependsOn<TModule>();
}