using Asp.Versioning;
using Carter;
using HomeInventory.Api;
using HomeInventory.Domain;
using HomeInventory.Modules;
using HomeInventory.Web.ErrorHandling;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.Metrics;

namespace HomeInventory.Tests.Systems.Modules;

public class BaseApiModuleGivenTestContext<TGiven, TModule> : GivenContext<TGiven, TModule>
    where TGiven : BaseApiModuleGivenTestContext<TGiven, TModule>
    where TModule : ApiCarterModule
{
    private readonly ModulesHost _host = new([new DomainModule(), new LoggingModule()]);
    private readonly IConfiguration _configuration = new ConfigurationManager();
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly Lazy<IServiceProvider> _lazyServiceProvider;
    private readonly IMetricsBuilder _metricsBuilder = Substitute.For<IMetricsBuilder>();

    protected BaseApiModuleGivenTestContext(BaseTest test)
        : base(test) =>
        _lazyServiceProvider = new(BuildServiceProvider);

    private IServiceProvider ServiceProvider => _lazyServiceProvider.Value;

    public async Task<TGiven> InitializeHostAsync()
    {
        await _host.AddServicesAsync(_services, _configuration, _metricsBuilder);
        return This;
    }

    public TGiven HttpContext(out IVariable<HttpContext> context) =>
        New(out context, CreateHttpContext);

    public TGiven DataSources(out IVariable<List<EndpointDataSource>> dataSources) =>
        New(out dataSources, static () => []);

    public TGiven RouteBuilder(out IVariable<IEndpointRouteBuilder> routeBuilder, IVariable<List<EndpointDataSource>> dataSources) =>
        SubstituteFor(out routeBuilder, dataSources, (b, s) =>
        {
            b.ServiceProvider.Returns(_ => ServiceProvider);
            b.DataSources.Returns(s);
        });

    protected override TModule CreateSut() => ServiceProvider.GetRequiredService<TModule>();

    protected virtual void AddServices(IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSingleton<IScopeFactory, ScopeFactory>()
            .AddSingleton<IScopeContainer, ScopeContainer>()
            .AddSingleton<IScopeAccessor, ScopeAccessor>()
            .AddOptions(new ApiVersioningOptions())
            .AddSubstitute<IReportApiVersions>()
            .AddSubstitute<IApiVersionParameterSource>()
            .AddSubstitute<IValidatorLocator>()
            .AddSingleton(ErrorMappingBuilder.CreateDefault().Build())
            .AddOptions(new ApiBehaviorOptions())
            .AddSingleton<HomeInventoryProblemDetailsFactory>()
            .AddSingleton<IProblemDetailsFactory>(static sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>())
            .AddSingleton(configuration)
            .AddSingleton<TModule>();

    private DefaultHttpContext CreateHttpContext() => new() { RequestServices = ServiceProvider };

    private ServiceProvider BuildServiceProvider()
    {
        AddServices(_services, _configuration);
        return _services.BuildServiceProvider();
    }
}
