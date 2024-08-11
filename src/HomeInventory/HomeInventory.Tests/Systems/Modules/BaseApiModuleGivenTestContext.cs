using Asp.Versioning;
using AutoMapper;
using Carter;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Web.Infrastructure;
using HomeInventory.Web.Modules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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

    public TGiven HttpContext(out IVariable<HttpContext> context) =>
        New(out context, CreateHttpContext);

    public TGiven DataSources(out IVariable<List<EndpointDataSource>> dataSources) =>
        New(out dataSources, () => new List<EndpointDataSource>());

    public TGiven RouteBuilder(out IVariable<IEndpointRouteBuilder> routeBuilder, IVariable<List<EndpointDataSource>> dataSources) =>
        SubstituteFor(out routeBuilder, dataSources, (b, s) =>
        {
            b.ServiceProvider.Returns(ServiceProvider);
            b.DataSources.Returns(s);
        });

    internal TGiven OnQueryReturn<TRequest, TResponse>(IVariable<TRequest> request, IVariable<TResponse> result)
        where TRequest : class, IRequestMessage<IQueryResult<TResponse>>
        where TResponse : notnull =>
        OnRequestReturnResult(request, result);

    internal TGiven OnCommandReturnSuccess<TRequest>(IVariable<TRequest> request)
        where TRequest : class, IRequestMessage<Option<Error>> =>
        OnRequestReturnResult(request);

    internal TGiven OnQueryReturnError<TRequest, TResponse, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : class, IRequestMessage<IQueryResult<TResponse>>
        where TResponse : notnull
        where TError : notnull, Error =>
        OnRequestReturnError<TRequest, TResponse, TError>(request, result);

    internal TGiven OnCommandReturnError<TRequest, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : class, IRequestMessage<Option<Error>>
        where TError : notnull, Error =>
    OnRequestReturnError<TRequest, TError>(request, result);

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
            _given._mapper.Map(sourceValue, Arg.Any<Action<IMappingOperationOptions<object, TDestination>>>()).Returns(destinationValue);

            return _given;
        }
    }

    protected override TModule CreateSut()
    {
        var accessor = ServiceProvider.GetRequiredService<IScopeAccessor>();
        accessor.Set(ServiceProvider.GetRequiredService<IMapper>());
        accessor.Set(ServiceProvider.GetRequiredService<IMessageHub>());
        accessor.Set(ServiceProvider.GetRequiredService<IProblemDetailsFactory>());
        return ServiceProvider.GetRequiredService<TModule>();
    }

    private DefaultHttpContext CreateHttpContext() => new() { RequestServices = ServiceProvider };

    private TGiven OnRequestReturnError<TRequest, TResponse, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : class, IRequestMessage<IQueryResult<TResponse>>
        where TResponse : notnull
        where TError : Error =>
        OnRequestReturn<TRequest, IQueryResult<TResponse>>(request, new QueryResult<TResponse>(GetValue(result)));

    private TGiven OnRequestReturnError<TRequest, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : class, IRequestMessage<Option<Error>>
        where TError : Error =>
        OnRequestReturn(request, Option<Error>.Some(GetValue(result)));

    private TGiven OnRequestReturnResult<TRequest, TResponse>(IVariable<TRequest> request, IVariable<TResponse> result)
        where TRequest : class, IRequestMessage<IQueryResult<TResponse>>
        where TResponse : notnull =>
        OnRequestReturnResult<TRequest, IQueryResult<TResponse>>(request, new QueryResult<TResponse>(GetValue(result)));

    private TGiven OnRequestReturnResult<TRequest>(IVariable<TRequest> request)
        where TRequest : class, IRequestMessage<Option<Error>> =>
        OnRequestReturnResult<TRequest, Option<Error>>(request, OptionNone.Default);

    private TGiven OnRequestReturnResult<TRequest, TResponse>(IVariable<TRequest> request, TResponse resultValue)
        where TRequest : class, IRequestMessage<TResponse>
        where TResponse : notnull =>
        OnRequestReturn(request, resultValue);

    private TGiven OnRequestReturn<TRequest, TResponse>(IVariable<TRequest> request, TResponse result)
        where TRequest : class, IRequestMessage<TResponse>
        where TResponse : notnull
    {
        var requestValue = GetValue(request);
        var subscription = Hub.GetMessages<CancellableRequest<TRequest, TResponse>>()
            .Where(cr => cr.Message == requestValue)
            .Take(1)
            .Subscribe(cr =>
            {
                var response = Hub.CreateMessage((id, on) => new ResposeMessage<TRequest, TResponse>(id, on, cr.Message, result));
                Hub.OnNext(response);
            });

        AddDisposable(subscription);
        return This;
    }

    private sealed class QueryResult<TResponse>(Validation<Error, TResponse> validation) : IQueryResult<TResponse>
        where TResponse : notnull
    {
        private readonly Validation<Error, TResponse> _validation = validation;

        public TResponse Success => (TResponse)_validation;
        public bool IsFail => _validation.IsFail;
        public bool IsSuccess => _validation.IsSuccess;
        public Seq<Error> Fail => (Seq<Error>)_validation;
        object IQueryResult.Success => Success;

        public bool Equals(TResponse other) => _validation == other;

        public Seq<Error> FailAsEnumerable() => _validation.FailAsEnumerable();

        public LanguageExt.Unit IfSuccess(Action<TResponse> Success) => _validation.IfSuccess(Success);

        public TResult Match<TResult>(Func<TResponse, TResult> Succ, Func<Seq<Error>, TResult> Fail) =>
            _validation.Match(Succ, Fail);

        public Seq<TResponse> SuccessAsEnumerable() => _validation.SuccessAsEnumerable();
    }
}
