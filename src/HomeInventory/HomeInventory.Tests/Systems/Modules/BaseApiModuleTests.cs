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
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Modules;

public abstract class BaseApiModuleTests : BaseApiModuleTests<BaseApiModuleTests.ApiGivenTestContext>
{
    protected override ApiGivenTestContext CreateGiven(VariablesCollection variables) =>
        new(variables, Fixture, Cancellation);

    public sealed class ApiGivenTestContext : BaseApiGivenTestContext
    {
        public ApiGivenTestContext(VariablesCollection variables, IFixture fixture, ICancellation cancellation)
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
        Fixture.CustomizeGuidId<UserId>();
        Fixture.CustomizeEmail();
        Fixture.CustomizeFromFactory<Guid, ISupplier<Guid>>(_ => new ValueSupplier<Guid>(Guid.NewGuid()));
    }

    public class BaseApiGivenTestContext : GivenContext<TGiven>
    {
        private readonly Variable<HttpContext> _context = new(nameof(_context));
        private readonly ISender _mediator = Substitute.For<ISender>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly ICancellation _cancellation;

        public BaseApiGivenTestContext(VariablesCollection variables, IFixture fixture, ICancellation cancellation)
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

        protected ICancellation Cancellation => _cancellation;

        protected ISender Mediator => _mediator;

        internal TGiven OnQueryReturn<TRequest, TResult>(Variable<TRequest> request, Variable<TResult> result)
            where TRequest : notnull, IQuery<TResult>
            where TResult : notnull
        {
            var requestValue = Variables.Get(request.WithIndex(0));
            var resultValue = Variables.Get(result.WithIndex(0));
            _mediator.Send(requestValue, _cancellation.Token).Returns(resultValue);
            return This;
        }

        internal TGiven OnCommandReturnSuccess<TRequest>(Variable<TRequest> request)
            where TRequest : notnull, ICommand
        {
            var requestValue = Variables.Get(request.WithIndex(0));
            _mediator.Send(requestValue, _cancellation.Token).Returns(new Success());
            return This;
        }

        internal TGiven OnQueryReturnError<TRequest, TResult, TError>(Variable<TRequest> request, Variable<TError> result)
            where TRequest : notnull, IQuery<TResult>
            where TResult : notnull
            where TError : notnull, IError
        {
            var requestValue = Variables.Get(request.WithIndex(0));
            var resultValue = Variables.Get(result.WithIndex(0));
            _mediator.Send(requestValue, _cancellation.Token).Returns(resultValue);
            return This;
        }

        internal TGiven OnCommandReturnError<TRequest, TError>(Variable<TRequest> request, Variable<TError> result)
            where TRequest : notnull, ICommand
            where TError : notnull, IError
        {
            var requestValue = Variables.Get(request.WithIndex(0));
            var resultValue = Variables.Get(result.WithIndex(0));
            _mediator.Send(requestValue, _cancellation.Token).Returns(resultValue);
            return This;
        }

        public TGiven Map<TSource, TDestination>(Variable<TSource> source, Variable<TDestination> destination)
            where TSource : notnull
            where TDestination : notnull
        {
            New(source);
            New(destination);
            _mapper.Map<TDestination>(Variables.Get(source.WithIndex(0)))
                .Returns(Variables.Get(destination.WithIndex(0)));
            return This;
        }
    }
}
