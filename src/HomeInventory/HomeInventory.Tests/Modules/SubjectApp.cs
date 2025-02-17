using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Tests.Modules;

public sealed class SubjectApp : IApplicationBuilder, IEndpointRouteBuilder
{
    public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware) => this;

    public IApplicationBuilder New() => this;

    public RequestDelegate Build() => _ => Task.CompletedTask;

    public IServiceProvider ApplicationServices { get; set; } = Substitute.For<IServiceProvider>();
    public IFeatureCollection ServerFeatures { get; } = Substitute.For<IFeatureCollection>();
    public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();
    public IApplicationBuilder CreateApplicationBuilder() => this;

    public IServiceProvider ServiceProvider { get; } = Substitute.For<IServiceProvider>();
    public ICollection<EndpointDataSource> DataSources { get; } = [];
}