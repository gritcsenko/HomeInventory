using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.Framework;

public abstract class BaseModuleWithMediatr : BaseModule, IModuleWithMediatr
{
    protected BaseModuleWithMediatr()
    {
        DependsOn<ApplicationMediatrSupportModule>();
    }

    public virtual void Configure(MediatRServiceConfiguration configuration)
    {
    }
}
