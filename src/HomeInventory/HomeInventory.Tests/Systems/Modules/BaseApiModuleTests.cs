using AutoMapper;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Modules;

public abstract class BaseApiModuleTests() : BaseApiModuleTests<BaseApiModuleTests.ApiGivenTestContext>(t => new(t))
{
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable S2094 // Classes should not be empty
    public sealed class ApiGivenTestContext(BaseTest test) : BaseApiGivenTestContext(test)
#pragma warning restore S2094 // Classes should not be empty
#pragma warning restore CA1034 // Nested types should not be visible
    {
    }
}

public abstract class BaseApiModuleTests<TGiven> : BaseTest<TGiven>
    where TGiven : BaseApiModuleTests<TGiven>.BaseApiGivenTestContext
{
    protected BaseApiModuleTests(Func<BaseTest, TGiven> createGiven)
        : base(createGiven)
    {
        Fixture
            .CustomizeUlidId<UserId>()
            .CustomizeEmail()
            .CustomizeFromFactory<Ulid, ISupplier<Ulid>>(_ => new ValueSupplier<Ulid>(Ulid.NewUlid()));
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public class BaseApiGivenTestContext : GivenContext<TGiven>
#pragma warning restore CA1034 // Nested types should not be visible
    {
        private readonly Variable<HttpContext> _context = new(nameof(_context));
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ICancellation _cancellation;

        public BaseApiGivenTestContext(BaseTest test)
            : base(test)
        {
            _cancellation = test.Cancellation;

            var collection = new ServiceCollection()
                .AddSubstitute(out _mediator)
                .AddSubstitute(out _mapper)
                .AddSingleton<ErrorMapping>()
                .AddOptions(new ApiBehaviorOptions())
                .AddSingleton<HomeInventoryProblemDetailsFactory>();

            Add(_context, () => new DefaultHttpContext
            {
                RequestServices = collection.BuildServiceProvider()
            });
        }

        public IVariable<HttpContext> Context => _context;

        internal TGiven OnQueryReturn<TRequest, TResult>(Variable<TRequest> request, Variable<TResult> result)
            where TRequest : notnull, IQuery<TResult>
            where TResult : notnull =>
            OnRequestReturnResult(request, result);

        internal TGiven OnCommandReturnSuccess<TRequest>(Variable<TRequest> request)
            where TRequest : notnull, ICommand =>
            OnRequestReturnResult(request, new Success());

        internal TGiven OnQueryReturnError<TRequest, TResult, TError>(Variable<TRequest> request, Variable<TError> result)
            where TRequest : notnull, IQuery<TResult>
            where TResult : notnull
            where TError : notnull, IError =>
            OnRequestReturnError<TRequest, TResult, TError>(request, result);

        internal TGiven OnCommandReturnError<TRequest, TError>(Variable<TRequest> request, Variable<TError> result)
            where TRequest : notnull, ICommand
            where TError : notnull, IError =>
            OnRequestReturnError<TRequest, Success, TError>(request, result);

        public TGiven Map<TSource, TDestination>(Variable<TSource> source, Variable<TDestination> destination)
            where TSource : notnull
            where TDestination : notnull
        {
            New(source);
            New(destination);
            var sourceValue = GetValue(source);
            var destinationValue = GetValue(destination);
            _mapper.Map<TDestination>(sourceValue).Returns(destinationValue);
            return This;
        }

        private TGiven OnRequestReturnError<TRequest, TResult, TError>(Variable<TRequest> request, Variable<TError> result)
            where TRequest : IRequest<OneOf<TResult, IError>>
            where TResult : notnull
            where TError : IError
        {
            var requestValue = GetValue(request);
            var resultValue = GetValue(result);
            _mediator.Send(requestValue, _cancellation.Token).Returns(OneOf<TResult, IError>.FromT1(resultValue));
            return This;
        }

        private TGiven OnRequestReturnResult<TRequest, TResult>(Variable<TRequest> request, Variable<TResult> result)
            where TRequest : IRequest<OneOf<TResult, IError>>
            where TResult : notnull =>
            OnRequestReturnResult(request, GetValue(result));

        private TGiven OnRequestReturnResult<TRequest, TResult>(Variable<TRequest> request, TResult resultValue)
            where TRequest : IRequest<OneOf<TResult, IError>>
            where TResult : notnull
        {
            var requestValue = GetValue(request);
            _mediator.Send(requestValue, _cancellation.Token).Returns(OneOf<TResult, IError>.FromT0(resultValue));
            return This;
        }
    }
}
