using Asp.Versioning;
using AutoMapper;
using Carter;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Web.Infrastructure;
using HomeInventory.Web.Modules;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Systems.Modules;

public class BaseApiModuleGivenTestContext<TGiven, TModule> : GivenContext<TGiven, TModule>
    where TGiven : BaseApiModuleGivenTestContext<TGiven, TModule>
    where TModule : ApiModule
{
    private readonly IServiceCollection _services;
    private readonly Lazy<IServiceProvider> _lazyServiceProvider;
    private readonly ISender _mediator;
    private readonly IMapper _mapper;
    private readonly ICancellation _cancellation;

    public BaseApiModuleGivenTestContext(BaseTest test)
        : base(test)
    {
        _cancellation = test.Cancellation;

        _services = new ServiceCollection()
            .AddDomain()
            .AddOptions(new ApiVersioningOptions())
            .AddSubstitute<IReportApiVersions>()
            .AddSubstitute<IApiVersionParameterSource>()
            .AddSubstitute<IValidatorLocator>()
            .AddSubstitute(out _mediator)
            .AddSubstitute(out _mapper)
            .AddSingleton(ErrorMappingBuilder.CreateDefault().Build())
            .AddOptions(new ApiBehaviorOptions())
            .AddSingleton<HomeInventoryProblemDetailsFactory>()
            .AddSingleton<IProblemDetailsFactory>(sp => sp.GetRequiredService<HomeInventoryProblemDetailsFactory>())
            .AddSingleton<TModule>();

        _lazyServiceProvider = new Lazy<IServiceProvider>(() => _services.BuildServiceProvider());
    }

    protected IServiceCollection Services => _services;

    protected IServiceProvider ServiceProvider => _lazyServiceProvider.Value;

    public TGiven HttpContext(out IVariable<HttpContext> context) =>
        New(out context, CreateHttpContext);

    public TGiven DataSources(out IVariable<List<EndpointDataSource>> dataSources) =>
        New(out dataSources, static () => new List<EndpointDataSource>());

    public TGiven RouteBuilder(out IVariable<IEndpointRouteBuilder> routeBuilder, IVariable<List<EndpointDataSource>> dataSources) =>
        SubstituteFor(out routeBuilder, dataSources, (b, s) =>
        {
            b.ServiceProvider.Returns(ServiceProvider);
            b.DataSources.Returns(s);
        });

    internal TGiven OnQueryReturn<TRequest, TResult>(IVariable<TRequest> request, IVariable<TResult> result)
        where TRequest : notnull, IQuery<TResult>
        where TResult : notnull =>
        OnRequestReturnResult(request, result);

    internal TGiven OnCommandReturnSuccess<TRequest>(IVariable<TRequest> request)
        where TRequest : notnull, ICommand =>
        OnRequestReturnResult(request);

    internal TGiven OnQueryReturnError<TRequest, TResult, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : notnull, IQuery<TResult>
        where TResult : notnull
        where TError : notnull, Error =>
        OnRequestReturnError<TRequest, TResult, TError>(request, result);

    internal TGiven OnCommandReturnError<TRequest, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : notnull, ICommand
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

            return _given;
        }
    }

    protected override TModule CreateSut() => ServiceProvider.GetRequiredService<TModule>();

    private DefaultHttpContext CreateHttpContext() => new() { RequestServices = ServiceProvider };

    private TGiven OnRequestReturnError<TRequest, TResult, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : IRequest<IQueryResult<TResult>>
        where TResult : notnull
        where TError : Error =>
        OnRequestReturn<TRequest, IQueryResult<TResult>>(request, new QueryResult<TResult>(GetValue(result)));

    private TGiven OnRequestReturnError<TRequest, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : IRequest<Option<Error>>
        where TError : Error =>
        OnRequestReturn(request, Option<Error>.Some(GetValue(result)));

    private TGiven OnRequestReturnResult<TRequest, TResult>(IVariable<TRequest> request, IVariable<TResult> result)
        where TRequest : IRequest<IQueryResult<TResult>>
        where TResult : notnull =>
        OnRequestReturnResult<TRequest, IQueryResult<TResult>>(request, new QueryResult<TResult>(GetValue(result)));

    private TGiven OnRequestReturnResult<TRequest>(IVariable<TRequest> request)
        where TRequest : IRequest<Option<Error>> =>
        OnRequestReturnResult<TRequest, Option<Error>>(request, OptionNone.Default);

    private TGiven OnRequestReturnResult<TRequest, TResult>(IVariable<TRequest> request, TResult resultValue)
        where TRequest : IRequest<TResult>
        where TResult : notnull =>
        OnRequestReturn(request, resultValue);

    private TGiven OnRequestReturn<TRequest, TResult>(IVariable<TRequest> request, TResult result)
        where TRequest : IRequest<TResult>
        where TResult : notnull
    {
        _mediator.Send(GetValue(request), _cancellation.Token)
            .Returns(result);
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
