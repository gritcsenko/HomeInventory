using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.Framework;

public abstract class BaseModuleWithMediatr : BaseModule, IModuleWithMediatr
{
    public abstract void Configure(MediatRServiceConfiguration configuration);

    protected void RegisterServicesFromCurrentAssembly(MediatRServiceConfiguration configuration)
    {
        configuration.RegisterServicesFromAssembly(GetType().Assembly);
    }
}
