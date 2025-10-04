using Asp.Versioning;
using AutoMapper;
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
using System.Runtime.CompilerServices;
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
    private readonly IMapper _mapper;
    private readonly IMetricsBuilder _metricsBuilder = Substitute.For<IMetricsBuilder>();

    protected BaseApiModuleGivenTestContext(BaseTest test)
        : base(test)
    {
        _services
            .AddSingleton<IScopeFactory, ScopeFactory>()
            .AddSingleton<IScopeContainer, ScopeContainer>()
            .AddSingleton<IScopeAccessor, ScopeAccessor>()
            .AddOptions(new ApiVersioningOptions())
            .AddSubstitute<IReportApiVersions>()
            .AddSubstitute<IApiVersionParameterSource>()
            .AddSubstitute<IValidatorLocator>()
            .AddSubstitute(out _mapper)
            .AddSingleton(ErrorMappingBuilder.CreateDefault().Build())
            .AddOptions(new ApiBehaviorOptions())
            .AddSingleton<HomeInventoryProblemDetailsFactory>()
            .AddSingleton<IProblemDetailsFactory>(static sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>())
            .AddSingleton(_configuration)
            .AddSingleton<TModule>();

        _lazyServiceProvider = new(_services.BuildServiceProvider);
    }

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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Code Smell", "S3427:Method overloads with default parameter values should not overlap", Justification = "False positive")]
    internal IDestinationMapper Map<TSource>(out IVariable<TSource> source, [CallerArgumentExpression(nameof(source))] string? name = null)
        where TSource : notnull =>
        New(out source, name: name).Map(source);

    internal IDestinationMapper Map<TSource>(IVariable<TSource> source)
        where TSource : notnull =>
        new DestinationMapper<TSource>(This, source);

    internal interface IDestinationMapper
    {
        TGiven To<TDestination>(out IVariable<TDestination> destination, [CallerArgumentExpression(nameof(destination))] string? name = null)
            where TDestination : notnull;
    }

    private sealed class DestinationMapper<TSource>(TGiven given, IVariable<TSource> source) : IDestinationMapper
        where TSource : notnull
    {
        private readonly TGiven _given = given;
        private readonly IVariable<TSource> _source = source;

        public TGiven To<TDestination>(out IVariable<TDestination> destination, [CallerArgumentExpression(nameof(destination))] string? name = null)
            where TDestination : notnull
        {
            _given.New(out destination, name: name);

            var sourceValue = _given.GetValue(_source);
            var destinationValue = _given.GetValue(destination);
            _given._mapper.Map<TDestination>(sourceValue).Returns(destinationValue);

            return _given;
        }
    }

    protected override TModule CreateSut() => ServiceProvider.GetRequiredService<TModule>();

    private DefaultHttpContext CreateHttpContext() => new() { RequestServices = ServiceProvider };
}
