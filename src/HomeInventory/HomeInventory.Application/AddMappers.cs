using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

internal class AddMappers : IStartupFilter
{
    private readonly IEnumerable<IMappingAssemblySource> _sources;

    public AddMappers(IEnumerable<IMappingAssemblySource> sources)
    {
        _sources = sources;
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        => builder =>
        {
            var config = builder.ApplicationServices.GetRequiredService<TypeAdapterConfig>();
            foreach (var source in _sources)
            {
                config.Scan(source.GetAssembly());
            }
            next(builder);
        };
}
