﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.DependencyInjection;

public class TestAppBuilder : IApplicationBuilder, IEndpointRouteBuilder
{
    private readonly ICollection<Func<RequestDelegate, RequestDelegate>> _middlewares = new List<Func<RequestDelegate, RequestDelegate>>();

    public TestAppBuilder(IServiceCollection collection, IServiceProviderFactory<IServiceCollection> factory)
    {
        var services = factory.CreateServiceProvider(collection);
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

