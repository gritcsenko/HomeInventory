using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using System.Collections;

namespace HomeInventory.Modules;

public class ModulesCollection : IReadOnlyCollection<IModule>
{
    private readonly List<IModule> _modules = [];

    public int Count => _modules.Count;

    public void Add(IModule module)
    {
        _modules.Add(module);
        if (module is IAttachableModule attachable)
        {
            attachable.OnAttached(this);
        }
    }

    public void InjectTo(IHostApplicationBuilder builder)
    {
        foreach (var module in _modules)
        {
            module.AddServices(builder.Services, builder.Configuration);
        }
    }

    public void BuildInto(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
        foreach (var module in _modules)
        {
            module.BuildApp(applicationBuilder, endpointRouteBuilder);
        }
    }

    public IEnumerator<IModule> GetEnumerator() => _modules.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
