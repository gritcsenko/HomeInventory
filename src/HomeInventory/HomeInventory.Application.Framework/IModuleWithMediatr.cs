using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.Framework;

public interface IModuleWithMediatr : IModule
{
    void Configure(MediatRServiceConfiguration configuration);
}
