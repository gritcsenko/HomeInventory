using Asp.Versioning;
using AutoMapper;
using Carter;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Web.Infrastructure;
using HomeInventory.Web.Modules;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

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

    public TGiven HttpContext(out IVariable<HttpContext> context)
    {
        context = new Variable<HttpContext>(nameof(context));
        return Add(context, CreateHttpContext);
    }

    public TGiven DataSources(out IVariable<List<EndpointDataSource>> dataSources)
    {
        dataSources = new Variable<List<EndpointDataSource>>(nameof(dataSources));
        return Add(dataSources, []);
    }

    public TGiven RouteBuilder(IVariable<List<EndpointDataSource>> dataSources, out IVariable<IEndpointRouteBuilder> routeBuilder)
    {
        return SubstituteFor(out routeBuilder, dataSources, (b, s) => {
            b.ServiceProvider.Returns(ServiceProvider);
            b.DataSources.Returns(s);
        });
    }

    internal TGiven OnQueryReturn<TRequest, TResult>(IVariable<TRequest> request, IVariable<TResult> result)
        where TRequest : notnull, IQuery<TResult>
        where TResult : notnull =>
        OnRequestReturnResult(request, result);

    internal TGiven OnCommandReturnSuccess<TRequest>(IVariable<TRequest> request)
        where TRequest : notnull, ICommand =>
        OnRequestReturnResult(request, new Success());

    internal TGiven OnQueryReturnError<TRequest, TResult, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : notnull, IQuery<TResult>
        where TResult : notnull
        where TError : notnull, IError =>
        OnRequestReturnError<TRequest, TResult, TError>(request, result);

    internal TGiven OnCommandReturnError<TRequest, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : notnull, ICommand
        where TError : notnull, IError =>
        OnRequestReturnError<TRequest, Success, TError>(request, result);

    internal IDestinationMapper Map<TSource>(out IVariable<TSource> source)
        where TSource : notnull =>
        New(out source).Map(source);

    internal IDestinationMapper Map<TSource>(IVariable<TSource> source)
        where TSource : notnull =>
        new DestinationMapper<TSource>(This, source);

    internal interface IDestinationMapper
    {
        TGiven To<TDestination>(out IVariable<TDestination> destination)
            where TDestination : notnull;
    }

    private sealed class DestinationMapper<TSource>(TGiven given, IVariable<TSource> source) : IDestinationMapper
        where TSource : notnull
    {
        private readonly TGiven _given = given;
        private readonly IVariable<TSource> _source = source;

        public TGiven To<TDestination>(out IVariable<TDestination> destination)
            where TDestination : notnull
        {
            _given.New(out destination);
            
            var sourceValue = _given.GetValue(_source);
            var destinationValue = _given.GetValue(destination);
            _given._mapper.Map<TDestination>(sourceValue).Returns(destinationValue);

            return _given;
        }
    }

    protected override TModule CreateSut() => ServiceProvider.GetRequiredService<TModule>();

    private DefaultHttpContext CreateHttpContext() => new() { RequestServices = ServiceProvider };

    private TGiven OnRequestReturnError<TRequest, TResult, TError>(IVariable<TRequest> request, IVariable<TError> result)
        where TRequest : IRequest<OneOf<TResult, IError>>
        where TResult : notnull
        where TError : IError =>
        OnRequestReturn(request, OneOf<TResult, IError>.FromT1(GetValue(result)));

    private TGiven OnRequestReturnResult<TRequest, TResult>(IVariable<TRequest> request, IVariable<TResult> result)
        where TRequest : IRequest<OneOf<TResult, IError>>
        where TResult : notnull =>
        OnRequestReturnResult(request, GetValue(result));

    private TGiven OnRequestReturnResult<TRequest, TResult>(IVariable<TRequest> request, TResult resultValue)
        where TRequest : IRequest<OneOf<TResult, IError>>
        where TResult : notnull =>
        OnRequestReturn(request, OneOf<TResult, IError>.FromT0(resultValue));

    private TGiven OnRequestReturn<TRequest, TResult>(IVariable<TRequest> request, OneOf<TResult, IError> oneOf)
        where TRequest : IRequest<OneOf<TResult, IError>>
        where TResult : notnull
    {
        _mediator.Send(GetValue(request), _cancellation.Token)
            .Returns(oneOf);
        return This;
    }
}
