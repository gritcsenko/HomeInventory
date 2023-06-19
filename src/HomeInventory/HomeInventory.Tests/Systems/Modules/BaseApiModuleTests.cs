using AutoMapper;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Modules;

public abstract class BaseApiModuleTests : BaseApiModuleTests<BaseApiModuleTests.ApiGivenTestContext>
{
    protected override ApiGivenTestContext CreateGiven(VariablesContainer variables) =>
        new(variables, Fixture, Cancellation);

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class ApiGivenTestContext : BaseApiGivenTestContext
#pragma warning restore CA1034 // Nested types should not be visible
    {
        public ApiGivenTestContext(VariablesContainer variables, IFixture fixture, ICancellation cancellation)
            : base(variables, fixture, cancellation)
        {
        }
    }
}

public abstract class BaseApiModuleTests<TGiven> : BaseTest<TGiven>
    where TGiven : BaseApiModuleTests<TGiven>.BaseApiGivenTestContext
{
    protected BaseApiModuleTests()
    {
        Fixture.CustomizeUlidId<UserId>();
        Fixture.CustomizeEmail();
        Fixture.CustomizeFromFactory<Ulid, ISupplier<Ulid>>(_ => new ValueSupplier<Ulid>(Ulid.NewUlid()));
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public class BaseApiGivenTestContext : GivenContext<TGiven>
#pragma warning restore CA1034 // Nested types should not be visible
    {
        private readonly Variable<HttpContext> _context = new(nameof(_context));
        private readonly ISender _mediator = Substitute.For<ISender>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly ICancellation _cancellation;

        public BaseApiGivenTestContext(VariablesContainer variables, IFixture fixture, ICancellation cancellation)
            : base(variables, fixture)
        {
            _cancellation = cancellation;

            var collection = new ServiceCollection();
            collection.AddSingleton(_mediator);
            collection.AddSingleton(_mapper);
            collection.AddSingleton(new HomeInventoryProblemDetailsFactory(new ErrorMapping(), Options.Create(new ApiBehaviorOptions())));

            Add(_context, () => new DefaultHttpContext
            {
                RequestServices = collection.BuildServiceProvider()
            });
        }

        public IIndexedVariable<HttpContext> Context => _context.WithIndex(0);

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
            var sourceValue = Variables.Get(source.WithIndex(0));
            var destinationValue = Variables.Get(destination.WithIndex(0));
            _mapper.Map<TDestination>(sourceValue).Returns(destinationValue);
            return This;
        }

        private TGiven OnRequestReturnError<TRequest, TResult, TError>(Variable<TRequest> request, Variable<TError> result)
            where TRequest : IRequest<OneOf<TResult, IError>>
            where TResult : notnull
            where TError : IError
        {
            var requestValue = Variables.Get(request.WithIndex(0));
            var resultValue = Variables.Get(result.WithIndex(0));
            _mediator.Send(requestValue, _cancellation.Token).Returns(OneOf<TResult, IError>.FromT1(resultValue));
            return This;
        }

        private TGiven OnRequestReturnResult<TRequest, TResult>(Variable<TRequest> request, Variable<TResult> result)
            where TRequest : IRequest<OneOf<TResult, IError>>
            where TResult : notnull =>
            OnRequestReturnResult(request, Variables.Get(result.WithIndex(0)));

        private TGiven OnRequestReturnResult<TRequest, TResult>(Variable<TRequest> request, TResult resultValue)
            where TRequest : IRequest<OneOf<TResult, IError>>
            where TResult : notnull
        {
            var requestValue = Variables.Get(request.WithIndex(0));
            _mediator.Send(requestValue, _cancellation.Token).Returns(OneOf<TResult, IError>.FromT0(resultValue));
            return This;
        }
    }
}
