using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.DependencyInjection;

public class TestAppBuilder : IApplicationBuilder, IEndpointRouteBuilder
{
    private readonly List<Func<RequestDelegate, RequestDelegate>> _middlewares = [];

    public TestAppBuilder(IServiceCollection collection)
    {
        var services = collection.BuildServiceProvider(new ServiceProviderOptions());
        ApplicationServices = services;
        ServiceProvider = services;
    }

    public IServiceProvider ApplicationServices { get; set; }
    public IFeatureCollection ServerFeatures { get; } = new FeatureCollection();
    public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();
    public IServiceProvider ServiceProvider { get; }
    public ICollection<EndpointDataSource> DataSources { get; } = new List<EndpointDataSource>();

    public RequestDelegate Build()
    {
        return (HttpContext ctx) => Task.CompletedTask;
    }

    public IApplicationBuilder CreateApplicationBuilder()
    {
        return this;
    }

    public IApplicationBuilder New()
    {
        return this;
    }

    public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }
}

