using Carter;
using FluentValidation;
using FluentValidation.Results;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Error = LanguageExt.Common.Error;

namespace HomeInventory.Tests.Presentation.Web;

[UnitTest]
public sealed class ValidationEndpointFilterTests() : BaseTest<ValidationEndpointFilterTestsGivenContext>(t => new(t))
{
    [Fact]
    public async Task InvokeAsync_Should_ReturnResult_When_NoArguments()
    {
        Given
            .SubstituteFor<object>(out var resultVar)
            .SubstituteFor<EndpointFilterInvocationContext>(out var contextVar)
            .SubstituteFor<IValidationContextFactory<Guid>>(out var contextFactoryVar)
            .SubstituteFor<IProblemDetailsFactory>(out var problemFactoryVar)
            .Sut(out var sutVar, contextFactoryVar, problemFactoryVar);
        
        var then = await When
            .InvokedAsync(sutVar, contextVar, resultVar, static async (sut, ctx, r, _) => (await sut.InvokeAsync(ctx, x =>
            {
                x.Should().BeSameAs(ctx);
                return ValueTask.FromResult<object?>(r);
            }))!);
        
        then
            .Result(resultVar, static (actual, expected) => actual.Should().BeSameAs(expected));
    }

    [Fact]
    public async Task InvokeAsync_Should_ValidateArgument()
    {
        Given
            .SubstituteFor<object>(out var resultVar)
            .New(out var argVar, Guid.NewGuid)
            .New<CancellationToken>(out var tokenVar, () => new(false))
            .SubstituteFor<IValidationContext>(out var validationContextVar)
            .SubstituteFor<IValidator, IValidationContext, CancellationToken>(out var validatorVar, validationContextVar, tokenVar, static (v, ctx, ct) => v.ValidateAsync(ctx, ct).Returns(new ValidationResult()))
            .SubstituteFor<IValidatorLocator, IValidator>(out var locatorVar, validatorVar, static (l, v) => l.GetValidator<Guid>().Returns(v))
            .New<IServiceProvider, IValidatorLocator>(out var serviceProviderVar, locatorVar, static locator =>
            {
                var services = new ServiceCollection();
                services.AddSingleton(locator);
                return services.BuildServiceProvider();
            })
            .SubstituteFor<HttpContext, IServiceProvider, CancellationToken>(out var httpContextVar, serviceProviderVar, tokenVar, static (ctx, provider, ct) =>
            {
                ctx.RequestServices.Returns(provider);
                ctx.RequestAborted.Returns(ct);
            })
            .SubstituteFor<EndpointFilterInvocationContext, Guid, HttpContext>(out var invocationContextVar, argVar, httpContextVar, static (ctx, arg, http) =>
            {
                ctx.Arguments.Returns([arg]);
                ctx.HttpContext.Returns(http);
            })
            .SubstituteFor<IValidationContextFactory<Guid>, Guid, IValidationContext>(out var contextFactoryVar, argVar, validationContextVar, static (f, arg, ctx) => f.CreateContext(arg).Returns(ctx))
            .SubstituteFor<IProblemDetailsFactory>(out var problemFactoryVar)
            .Sut(out var sutVar, contextFactoryVar, problemFactoryVar);
        
        var then = await When
            .InvokedAsync(sutVar, invocationContextVar, resultVar, static async (sut, ctx, r, _) => (await sut.InvokeAsync(ctx, _ => ValueTask.FromResult<object?>(r)))!);
        
        then
            .Result(resultVar, static (actual, expected) => actual.Should().BeSameAs(expected))
            .Ensure(validatorVar, validationContextVar, tokenVar, static (v, ctx, ct) => v.Received(1).ValidateAsync(ctx, ct));
    }
    
    [Fact]
    public async Task InvokeAsync_Should_ReturnProblem_When_ArgumentIsInvalid()
    {
        Given
            .SubstituteFor<object>(out var resultVar)
            .New<string>(out var traceIdVar)
            .New(out var argVar, Guid.NewGuid)
            .New<CancellationToken>(out var tokenVar, () => new(false))
            .SubstituteFor<IValidationContext>(out var validationContextVar)
            .New<ValidationFailure>(out var failureVar)
            .SubstituteFor<IValidator, IValidationContext, CancellationToken, ValidationFailure>(out var validatorVar, validationContextVar, tokenVar, failureVar, static (v, ctx, ct, f) => v.ValidateAsync(ctx, ct).Returns(new ValidationResult([f])))
            .SubstituteFor<IValidatorLocator, IValidator>(out var locatorVar, validatorVar, static (l, v) => l.GetValidator<Guid>().Returns(v))
            .New<IServiceProvider, IValidatorLocator>(out var serviceProviderVar, locatorVar, static locator =>
            {
                var services = new ServiceCollection();
                services.AddSingleton(locator);
                return services.BuildServiceProvider();
            })
            .SubstituteFor<HttpContext, IServiceProvider, CancellationToken, string>(out var httpContextVar, serviceProviderVar, tokenVar, traceIdVar, static (ctx, provider, ct, trace) =>
            {
                ctx.RequestServices.Returns(provider);
                ctx.RequestAborted.Returns(ct);
                ctx.TraceIdentifier.Returns(trace);
            })
            .SubstituteFor<EndpointFilterInvocationContext, Guid, HttpContext>(out var invocationContextVar, argVar, httpContextVar, static (ctx, arg, http) =>
            {
                ctx.Arguments.Returns([arg]);
                ctx.HttpContext.Returns(http);
            })
            .SubstituteFor<IValidationContextFactory<Guid>, Guid, IValidationContext>(out var contextFactoryVar, argVar, validationContextVar, static (f, arg, ctx) => f.CreateContext(arg).Returns(ctx))
            .New<ProblemDetails>(out var detailsVar)
            .SubstituteFor<IProblemDetailsFactory, ValidationFailure, string, ProblemDetails>(out var problemFactoryVar, failureVar, traceIdVar, detailsVar, static (p, f, t, d) =>
                p.ConvertToProblem(Arg.Is((IReadOnlyCollection<Error> c) => c.Count == 1 && c.OfType<ValidationError>().Single().Message == f.ErrorMessage && c.OfType<ValidationError>().Single().Value == f.AttemptedValue), t).Returns(d))
            .Sut(out var sutVar, contextFactoryVar, problemFactoryVar);
        
        var then = await When
            .InvokedAsync(sutVar, invocationContextVar, resultVar, static async (sut, ctx, r, _) => (await sut.InvokeAsync(ctx, _ => ValueTask.FromResult<object?>(r)))!);
        
        then
            .Result(detailsVar, static (actual, expected) => actual.Should().BeOfType<ProblemHttpResult>().Which.ProblemDetails.Should().BeSameAs(expected))
            .Ensure(validatorVar, validationContextVar, tokenVar, static (v, ctx, ct) => v.Received(1).ValidateAsync(ctx, ct));
    }
}
