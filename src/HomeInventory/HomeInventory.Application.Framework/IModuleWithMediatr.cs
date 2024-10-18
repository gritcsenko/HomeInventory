using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HomeInventory.Application.Framework;

public interface IModuleWithMediatr : IModule
{
    void Configure(MediatRServiceConfiguration configuration);
}
