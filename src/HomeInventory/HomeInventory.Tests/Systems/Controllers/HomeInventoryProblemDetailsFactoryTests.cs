using System.Diagnostics;
using HomeInventory.Domain.Errors;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace HomeInventory.Tests.Systems.Controllers;

[UnitTest]
public class HomeInventoryProblemDetailsFactoryTests : BaseTest
{
    private readonly ApiBehaviorOptions _options;
    private readonly IOptions<ApiBehaviorOptions> _optionsAccessor;
    private readonly HttpContext _context;
    private readonly string _title;
    private readonly string _type;
    private readonly string _detail;
    private readonly string _instance;
    private readonly ModelStateDictionary _state;

    public HomeInventoryProblemDetailsFactoryTests()
    {
        _options = Fixture.Build<ApiBehaviorOptions>()
            .Without(x => x.InvalidModelStateResponseFactory)
            .Create();
        _optionsAccessor = Options.Create(_options);
        _context = Substitute.For<HttpContext>();
        _title = Fixture.Create<string>();
        _type = Fixture.Create<string>();
        _detail = Fixture.Create<string>();
        _instance = Fixture.Create<string>();
        _state = Fixture.Create<ModelStateDictionary>();

    }

    [Fact]
    public void CreateProblemDetails_Should_SetDefaultStatusTo500()
    {
        var sut = CreateSut();

        var details = sut.CreateProblemDetails(_context, statusCode: null);

        details.Should().NotBeNull();
        details.Status.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public void CreateProblemDetails_Should_PassValues()
    {
        var sut = CreateSut();
        var statusCode = StatusCodes.Status501NotImplemented;

        var details = sut.CreateProblemDetails(_context, statusCode, _title, _type, _detail, _instance);

        details.Should().NotBeNull();
        details.Status.Should().Be(statusCode);
        details.Title.Should().Be(_title);
        details.Type.Should().Be(_type);
        details.Detail.Should().Be(_detail);
        details.Instance.Should().Be(_instance);
    }

    [Fact]
    public void CreateProblemDetails_Should_SetDefaultsForStatusCode()
    {
        var sut = CreateSut();
        var statusCode = StatusCodes.Status501NotImplemented;
        _options.ClientErrorMapping[statusCode] = new ClientErrorData { Title = _title, Link = _type };

        var details = sut.CreateProblemDetails(_context, statusCode, title: null, type: null);

        details.Should().NotBeNull();
        details.Status.Should().Be(statusCode);
        details.Title.Should().Be(_title);
        details.Type.Should().Be(_type);
    }

    [Fact]
    public void CreateProblemDetails_Should_SetTraceIdentifierFromCurrentActivity()
    {
        var sut = CreateSut();
        var activity = new Activity("Testing");
        activity.Start();
        var id = activity.Id;

        var details = sut.CreateProblemDetails(_context);
        activity.Stop();

        details.Should().NotBeNull();
        details.Extensions.Should().ContainKey("traceId")
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(id);
    }

    [Fact]
    public void CreateProblemDetails_Should_SetTraceIdentifierFromContextWhenNoCurrentActivity()
    {
        var sut = CreateSut();
        var id = Fixture.Create<string>();
        _context.TraceIdentifier.Returns(id);

        var details = sut.CreateProblemDetails(_context);

        Activity.Current.Should().BeNull();
        details.Should().NotBeNull();
        details.Extensions.Should().ContainKey("traceId")
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(id);
    }

    [Fact]
    public void CreateProblemDetails_Should_SetErrorCodesIfProvidedViaContext()
    {
        var sut = CreateSut();
        var errors = new[] { new DuplicateEmailError() };
        _context.Items.Returns(new Dictionary<object, object?> { ["Errors"] = errors });

        var details = sut.CreateProblemDetails(_context);

        details.Should().NotBeNull();
        details.Extensions.Should().ContainKey("problems")
            .WhoseValue.Should().BeAssignableTo<IEnumerable<ProblemDetails>>()
            .Which.Select(p => p.Title)
            .Should().BeEquivalentTo(new[] { errors[0].GetType().Name });
    }

    [Fact]
    public void CreateValidationProblemDetails_Should_SetDefaultStatusTo400()
    {
        var sut = CreateSut();

        var details = sut.CreateValidationProblemDetails(_context, _state, statusCode: null);

        details.Should().NotBeNull();
        details.Status.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public void CreateValidationProblemDetails_Should_PassValues()
    {
        var sut = CreateSut();
        var state = new ModelStateDictionary();
        var key = Fixture.Create<string>();
        var errorMessage = Fixture.Create<string>();
        state.AddModelError(key, errorMessage);
        var statusCode = StatusCodes.Status402PaymentRequired;

        var details = sut.CreateValidationProblemDetails(_context, state, statusCode, _title, _type, _detail, _instance);

        details.Should().NotBeNull();
        details.Status.Should().Be(statusCode);
        details.Title.Should().Be(_title);
        details.Type.Should().Be(_type);
        details.Detail.Should().Be(_detail);
        details.Instance.Should().Be(_instance);
        details.Errors.Should().ContainKey(key)
            .WhoseValue.Should().BeEquivalentTo(new[] { errorMessage });
    }

    [Fact]
    public void CreateValidationProblemDetails_Should_SetDefaultsForStatusCode()
    {
        var sut = CreateSut();
        var statusCode = StatusCodes.Status402PaymentRequired;
        var title = new HttpValidationProblemDetails().Title;
        _options.ClientErrorMapping[statusCode] = new ClientErrorData { Title = Fixture.Create<string>(), Link = _type };

        var details = sut.CreateValidationProblemDetails(_context, _state, statusCode, title: null, type: null);

        details.Should().NotBeNull();
        details.Status.Should().Be(statusCode);
        details.Title.Should().Be(title);
        details.Type.Should().Be(_type);
    }

    [Fact]
    public void CreateValidationProblemDetails_Should_SetTraceIdentifierFromCurrentActivity()
    {
        var sut = CreateSut();
        var activity = new Activity("Testing");
        activity.Start();
        var id = activity.Id;

        var details = sut.CreateValidationProblemDetails(_context, _state);
        activity.Stop();

        details.Should().NotBeNull();
        details.Extensions.Should().ContainKey("traceId")
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(id);
    }

    [Fact]
    public void CreateValidationProblemDetails_Should_SetTraceIdentifierFromContextWhenNoCurrentActivity()
    {
        var sut = CreateSut();
        var id = Fixture.Create<string>();
        _context.TraceIdentifier.Returns(id);

        var details = sut.CreateValidationProblemDetails(_context, _state);

        Activity.Current.Should().BeNull();
        details.Should().NotBeNull();
        details.Extensions.Should().ContainKey("traceId")
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(id);
    }

    [Fact]
    public void CreateValidationProblemDetails_Should_SetErrorCodesIfProvidedViaContext()
    {
        var sut = CreateSut();
        var errors = new[] { new DuplicateEmailError() };
        _context.Items.Returns(new Dictionary<object, object?> { ["Errors"] = errors });

        var details = sut.CreateValidationProblemDetails(_context, _state);

        details.Should().NotBeNull();
        details.Extensions.Should().ContainKey("problems")
            .WhoseValue.Should().BeAssignableTo<IEnumerable<ProblemDetails>>()
            .Which.Select(p => p.Title)
            .Should().BeEquivalentTo(new[] { errors[0].GetType().Name });
    }

    private HomeInventoryProblemDetailsFactory CreateSut() => new(_optionsAccessor);
}
