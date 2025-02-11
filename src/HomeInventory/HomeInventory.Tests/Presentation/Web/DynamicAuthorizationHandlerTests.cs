using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace HomeInventory.Tests.Presentation.Web;

[UnitTest]
public sealed class DynamicAuthorizationHandlerTests() : BaseTest<DynamicAuthorizationHandlerTestsGivenContext>(t => new(t))
{
    [Fact]
    public async Task HandleRequirementAsync_ShouldFail_WhenContextResourceIsNotHttpContext()
    {
        Given
            .Null<HttpContext>(out var httpContextVar)
            .Requirement(out var requirementVar)
            .Context(out var contextVar, requirementVar, httpContextVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, context, _) => sut.HandleAsync(context));

        then
            .Ensure(contextVar, static context =>
            {
                context.HasFailed.Should().BeTrue();
                context.FailureReasons.Should().ContainSingle()
                    .Which.Message.Should().Be("Context has invalid resource");
            });
    }

    [Fact]
    public async Task HandleRequirementAsync_ShouldFail_WhenHttpContextDoesNotHaveEndpoint()
    {
        Given
            .HttpContext(out var httpContextVar)
            .Requirement(out var requirementVar)
            .Context(out var contextVar, requirementVar, httpContextVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, context, _) => sut.HandleAsync(context));

        then
            .Ensure(contextVar, static context =>
            {
                context.HasFailed.Should().BeTrue();
                context.FailureReasons.Should().ContainSingle()
                    .Which.Message.Should().Be("HTTP context has ho endpoint");
            });
    }

    [Fact]
    public async Task HandleRequirementAsync_ShouldFail_WhenUserDoesNotHaveNameIdentifier()
    {
        Given
            .Endpoint(out var endpointVar)
            .HttpContext(out var httpContextVar, endpointVar)
            .Requirement(out var requirementVar)
            .Context(out var contextVar, requirementVar, httpContextVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, context, _) => sut.HandleAsync(context));

        then
            .Ensure(contextVar, static context =>
            {
                context.HasFailed.Should().BeTrue();
                context.FailureReasons.Should().ContainSingle()
                    .Which.Message.Should().Be($"User has no {ClaimTypes.NameIdentifier} claim");
            });
    }

    [Fact]
    public async Task HandleRequirementAsync_ShouldFail_WhenUserHaveWrongNameIdentifierFormat()
    {
        Given
            .New<string>(out var idVar, static () => $"{Guid.NewGuid()}|{Guid.NewGuid()}")
            .Endpoint(out var endpointVar)
            .HttpContext(out var httpContextVar, endpointVar)
            .Requirement(out var requirementVar)
            .Context(out var contextVar, requirementVar, httpContextVar, idVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, context, _) => sut.HandleAsync(context));

        then
            .Ensure(contextVar, idVar, static (context, id) =>
            {
                context.HasFailed.Should().BeTrue();
                context.FailureReasons.Should().ContainSingle()
                    .Which.Message.Should().Be($"User has {ClaimTypes.NameIdentifier} = '{id}' with unknown format");
            });
    }

    [Fact]
    public async Task HandleRequirementAsync_ShouldFail_WhenUserHaveWrongNameIdentifier()
    {
        Given
            .New(out var idVar, static () => Ulid.Empty.ToString())
            .Endpoint(out var endpointVar)
            .HttpContext(out var httpContextVar, endpointVar)
            .Requirement(out var requirementVar)
            .Context(out var contextVar, requirementVar, httpContextVar, idVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, context, _) => sut.HandleAsync(context));

        then
            .Ensure(contextVar, static context =>
            {
                context.HasFailed.Should().BeTrue();
                context.FailureReasons.Should().ContainSingle()
                    .Which.Message.Should().Be("Id has wrong value");
            });
    }

    [Fact]
    public async Task HandleRequirementAsync_ShouldFail_WhenEndpointHasNoPermissions()
    {
        Given
            .SubstituteFor<IUserRepository>(out var repositoryVar)
            .New(out var idVar, static () => Ulid.NewUlid().ToString())
            .Endpoint(out var endpointVar)
            .HttpContext(out var httpContextVar, endpointVar, repositoryVar, Cancellation)
            .Requirement(out var requirementVar)
            .Context(out var contextVar, requirementVar, httpContextVar, idVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, context, _) => sut.HandleAsync(context));

        then
            .Ensure(contextVar, static context =>
            {
                context.HasFailed.Should().BeTrue();
                context.FailureReasons.Should().ContainSingle()
                    .Which.Message.Should().Be("User has no permission");
            });
    }

    [Fact]
    public async Task HandleRequirementAsync_ShouldFail_WhenUserHasNoPermissions()
    {
        Given
            .New(out var permissionVar, () => PermissionType.ReadPermission)
            .SubstituteFor<IUserRepository>(out var repositoryVar)
            .New(out var idVar, static () => Ulid.NewUlid().ToString())
            .Endpoint(out var endpointVar)
            .HttpContext(out var httpContextVar, endpointVar, repositoryVar, Cancellation)
            .Requirement(out var requirementVar, endpointVar, permissionVar)
            .Context(out var contextVar, requirementVar, httpContextVar, idVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, context, _) => sut.HandleAsync(context));

        then
            .Ensure(contextVar, static context =>
            {
                context.HasFailed.Should().BeTrue();
                context.FailureReasons.Should().ContainSingle()
                    .Which.Message.Should().Be("User has no permission");
            });
    }

    [Fact]
    public async Task HandleRequirementAsync_ShouldSucceed_WhenUserHasPermission()
    {
        Given
            .New(out var permissionVar, () => PermissionType.ReadPermission)
            .New(out var idVar, static () => Ulid.NewUlid().ToString())
            .SubstituteFor<IUserRepository, string, PermissionType>(out var repositoryVar, idVar, permissionVar, (r, id, p) => r.HasPermissionAsync(new(Ulid.Parse(id, CultureInfo.InvariantCulture)), p.ToString(), Cancellation.Token).Returns(true))
            .Endpoint(out var endpointVar)
            .HttpContext(out var httpContextVar, endpointVar, repositoryVar, Cancellation)
            .Requirement(out var requirementVar, endpointVar, permissionVar)
            .Context(out var contextVar, requirementVar, httpContextVar, idVar)
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, context, _) => sut.HandleAsync(context));

        then
            .Ensure(contextVar, requirementVar, static (context, requirement) =>
            {
                context.HasSucceeded.Should().BeTrue();
                context.PendingRequirements.Should().NotContain(requirement);
            });
    }
}

public sealed class DynamicAuthorizationHandlerTestsGivenContext(BaseTest test) : GivenContext<DynamicAuthorizationHandlerTestsGivenContext, DynamicAuthorizationHandler>(test)
{
    public DynamicAuthorizationHandlerTestsGivenContext Endpoint(out IVariable<Endpoint> variable, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, 1, name);

    public DynamicAuthorizationHandlerTestsGivenContext HttpContext(out IVariable<HttpContext?> variable, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, static () =>
        {
            var context = Substitute.For<HttpContext>();
            return context;
        }, name: name);

    public DynamicAuthorizationHandlerTestsGivenContext HttpContext(out IVariable<HttpContext?> variable, IVariable<Endpoint> endpointVariable, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, () =>
        {
            var endpoint = GetValue(endpointVariable);
            var feature = Substitute.For<IEndpointFeature>();
            feature.Endpoint.Returns(endpoint);
            var features = Substitute.For<IFeatureCollection>();
            features.Get<IEndpointFeature>().Returns(feature);
            var context = Substitute.For<HttpContext>();

            var services = new ServiceCollection();

            context.Features.Returns(features);
            context.RequestServices.Returns(services.BuildServiceProvider());
            return context;
        }, name: name);

    public DynamicAuthorizationHandlerTestsGivenContext HttpContext(out IVariable<HttpContext?> variable, IVariable<Endpoint> endpointVariable, IVariable<IUserRepository> repositoryVariable, ICancellation cancellation, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, () =>
        {
            var endpoint = GetValue(endpointVariable);
            var feature = Substitute.For<IEndpointFeature>();
            feature.Endpoint.Returns(endpoint);
            var features = Substitute.For<IFeatureCollection>();
            features.Get<IEndpointFeature>().Returns(feature);
            var context = Substitute.For<HttpContext>();

            var services = new ServiceCollection();
            services.AddScoped(_ => GetValue(repositoryVariable));

            context.Features.Returns(features);
            context.RequestServices.Returns(services.BuildServiceProvider());
            context.RequestAborted.Returns(cancellation.Token);
            return context;
        }, name: name);

    public DynamicAuthorizationHandlerTestsGivenContext Requirement(out IVariable<DynamicPermissionRequirement> variable, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, count, name);

    public DynamicAuthorizationHandlerTestsGivenContext Requirement(out IVariable<DynamicPermissionRequirement> variable, IVariable<Endpoint> endpointVariable, IVariable<PermissionType> permissionVariable, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, endpointVariable, permissionVariable, (endpoint, permission) => new(e => e == endpoint ? [permission] : []), 1, name);

    public DynamicAuthorizationHandlerTestsGivenContext Context(out IVariable<AuthorizationHandlerContext> variable, IVariable<DynamicPermissionRequirement> requirementVariable, IVariable<HttpContext?> resourceVariable, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, requirementVariable, resourceVariable, (req, res) => new([req], new([new()]), res), count, name);

    public DynamicAuthorizationHandlerTestsGivenContext Context(out IVariable<AuthorizationHandlerContext> variable, IVariable<DynamicPermissionRequirement> requirementVariable, IVariable<HttpContext?> resourceVariable, IVariable<string> idVariable, int count = 1, [CallerArgumentExpression(nameof(variable))] string? name = null) =>
        New(out variable, requirementVariable, resourceVariable, (req, res) => new([req], new([new([new(ClaimTypes.NameIdentifier, GetValue(idVariable))])]), res), count, name);

    protected override DynamicAuthorizationHandler CreateSut() => new();
}