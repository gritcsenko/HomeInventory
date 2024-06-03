using Asp.Versioning;
using AutoMapper;
using Carter;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Web.Infrastructure;
using HomeInventory.Web.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OneOf;
using OneOf.Types;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Systems.Modules;

public class BaseApiModuleGivenTestContext<TGiven, TModule> : GivenContext<TGiven, TModule>
    where TGiven : BaseApiModuleGivenTestContext<TGiven, TModule>
    where TModule : ApiModule
{
    private readonly IServiceCollection _services;
    private readonly Lazy<IServiceProvider> _lazyServiceProvider;
    private readonly IMapper _mapper;

    public BaseApiModuleGivenTestContext(BaseTest test)
        : base(test)
    {
        _services = new ServiceCollection()
            .AddDomain()
            .AddOptions(new ApiVersioningOptions())
            .AddSubstitute<IReportApiVersions>()
            .AddSubstitute<IApiVersionParameterSource>()
            .AddSubstitute<IValidatorLocator>()
            .AddMessageHubCore()
            .AddSubstitute(out _mapper)
            .AddSingleton(ErrorMappingBuilder.CreateDefault().Build())
            .AddOptions(new ApiBehaviorOptions())
            .AddSingleton<HomeInventoryProblemDetailsFactory>()
            .AddSingleton<IProblemDetailsFactory>(sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>())
            .AddSingleton<TModule>();

        _lazyServiceProvider = new Lazy<IServiceProvider>(() => _services.BuildServiceProvider());
    }

    protected IServiceProvider ServiceProvider => _lazyServiceProvider.Value;

    protected IMessageHub Hub => ServiceProvider.GetRequiredService<IMessageHub>();

    public TGiven DataSources(out IVariable<List<EndpointDataSource>> dataSources) =>
        New(out dataSources, () => new List<EndpointDataSource>());

    public TGiven RouteBuilder(out IVariable<IEndpointRouteBuilder> routeBuilder, IVariable<List<EndpointDataSource>> dataSources) =>
        SubstituteFor(out routeBuilder, dataSources, (b, s) =>
        {
            b.ServiceProvider.Returns(ServiceProvider);
            b.DataSources.Returns(s);
        });

    internal TGiven OnQueryReturn<TRequest, TResult>(IVariable<TRequest> request, IVariable<TResult> result)
        where TRequest : notnull, IRequestMessage<TResult>
        where TResult : notnull =>
        OnRequestReturnResult(request, result);

    internal TGiven OnCommandReturnSuccess<TRequest>(IVariable<TRequest> request)
        where TRequest : notnull, IRequestMessage =>
        OnRequestReturnResult(request, new Success());

    internal TGiven OnQueryReturnError<TRequest, TResult, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : notnull, IRequestMessage<TResult>
        where TResult : notnull
        where TError : notnull, IError =>
        OnRequestReturnError<TRequest, TResult, TError>(request, result);

    internal TGiven OnCommandReturnError<TRequest, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : notnull, IRequestMessage
        where TError : notnull, IError =>
    OnRequestReturnError<TRequest, Success, TError>(request, result);

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

    private TGiven OnRequestReturnError<TRequest, TResponse, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : IRequestMessage<TResponse>
        where TResponse : notnull
        where TError : IError =>
        OnRequestReturn(request, OneOf<TResponse, IError>.FromT1(GetValue(result)));

    private TGiven OnRequestReturnResult<TRequest, TResponse>(IVariable<TRequest> request, IVariable<TResponse> result)
        where TRequest : IRequestMessage<TResponse>
        where TResponse : notnull =>
        OnRequestReturnResult(request, GetValue(result));

    private TGiven OnRequestReturnResult<TRequest, TResponse>(IVariable<TRequest> request, TResponse resultValue)
        where TRequest : IRequestMessage<TResponse>
        where TResponse : notnull =>
        OnRequestReturn(request, OneOf<TResponse, IError>.FromT0(resultValue));

    private TGiven OnRequestReturn<TRequest, TResponse>(IVariable<TRequest> request, OneOf<TResponse, IError> oneOf)
        where TRequest : IRequestMessage<TResponse>
        where TResponse : notnull
    {
        var requestValue = GetValue(request);
        var response = Hub.CreateMessage((id, on) => new ResposeMessage<TRequest, TResponse>(id, on, requestValue, oneOf));
        Hub.Inject(Observable.Return(requestValue));
        Hub.Inject(Observable.Return(response));
        return This;
    }
}
