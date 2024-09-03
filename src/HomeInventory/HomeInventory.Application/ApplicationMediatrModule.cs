using HomeInventory.Application.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public sealed class ApplicationMediatrModule : BaseModuleWithMediatr
{
    public override void Configure(MediatRServiceConfiguration configuration)
    {
        RegisterServicesFromCurrentAssembly(configuration);
    }
}
