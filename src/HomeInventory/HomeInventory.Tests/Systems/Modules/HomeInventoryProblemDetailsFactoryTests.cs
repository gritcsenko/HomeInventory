using System.Diagnostics;
using FluentAssertions.Execution;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public class HomeInventoryProblemDetailsFactoryTests : BaseTest
{
    private readonly ApiBehaviorOptions _options;
    private readonly HttpContext _context = new DefaultHttpContext();
    private readonly string _title;
    private readonly string _type;
    private readonly string _detail;
    private readonly string _instance;
    private readonly ModelStateDictionary _state;
    private readonly ErrorMapping _mapping = ErrorMappingBuilder.CreateDefault().Build();

    public HomeInventoryProblemDetailsFactoryTests()
    {
        Fixture.Customize(new ApiBehaviorOptionsCustomization());
        _options = Fixture.Create<ApiBehaviorOptions>();
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

        using var scope = new AssertionScope();
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

        using var scope = new AssertionScope();
        details.Status.Should().Be(statusCode);
        details.Title.Should().Be(_title);
        details.Type.Should().Be(_type);
    }

    [Fact]
    public void CreateProblemDetails_Should_SetTraceIdentifierFromCurrentActivity()
    {
        var sut = CreateSut();
        using var activity = new Activity("Testing");
        activity.Start();
        var id = activity.Id;

        var details = sut.CreateProblemDetails(_context);
        activity.Stop();

        using var scope = new AssertionScope();
        details.Extensions.Should().ContainKey("traceId")
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(id);
    }

    [Fact]
    public void CreateProblemDetails_Should_SetTraceIdentifierFromContextWhenNoCurrentActivity()
    {
        var sut = CreateSut();
        var id = Fixture.Create<string>();
        _context.TraceIdentifier = id;

        var details = sut.CreateProblemDetails(_context);

        using var scope = new AssertionScope();
        Activity.Current.Should().BeNull();
        details.Extensions.Should().ContainKey("traceId")
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(id);
    }

    [Fact]
    public void ConvertToProblem_Should_ThrowInvalidOperationException_When_NoErrors()
    {
        var sut = CreateSut();
        var errors = Seq<Error>.Empty;

        Action action = () => sut.ConvertToProblem(errors);

        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void ConvertToProblem_Should_Convert_When_SingleError()
    {
        var sut = CreateSut();
        var expectedDetail = Fixture.Create<string>();
        var error = new ValidationError(expectedDetail, Fixture.Create<object>());
        var metadata = new Dictionary<string, object?>
        {
            ["errorCodes"] = new[] { error.GetType().Name },
            ["errors"] = new Error[] { error },
        };
        var errors = Seq<Error>.Empty.Add(error);
        var errorType = error.GetType();
        var expectedStatus = (int)_mapping.GetError(errorType);
        var expectedTitle = errorType.Name;

        var details = sut.ConvertToProblem(errors);

        using var scope = new AssertionScope();
        details.Status.Should().Be(expectedStatus);
        details.Title.Should().Be(expectedTitle);
        details.Type.Should().BeNull();
        details.Detail.Should().Be(expectedDetail);
        details.Instance.Should().BeNull();
        foreach (var (key, value) in metadata)
        {
            details.Extensions.Should().ContainKey(key)
                .WhoseValue.Should().BeEquivalentTo(value);
        }
    }

    [Fact]
    public void ConvertToProblem_Should_Convert_When_MultipleDifferentErrors()
    {
        var sut = CreateSut();
        var messages = Fixture.CreateMany<string>(2).ToArray();
        var metadata = Fixture.Create<Dictionary<string, object?>>();
        var errors = Seq<Error>.Empty.Add(new ValidationError(messages[0], metadata)).Add(new ConflictError(messages[1]));
        var expectedStatus = (int)_mapping.GetDefaultError();

        var details = sut.ConvertToProblem(errors);

        details.Should().NotBeNull();
        details.Status.Should().Be(expectedStatus);
        details.Title.Should().Be("Multiple Problems");
        details.Type.Should().BeNull();
        details.Detail.Should().Be("There were multiple problems that have occurred.");
        details.Instance.Should().BeNull();
        details.Extensions.Should().ContainKey("problems")
            .WhoseValue.Should().BeAssignableTo<ProblemDetails[]>()
            .Which.Should().HaveSameCount(errors);
    }

    [Fact]
    public void ConvertToProblem_Should_ConvertWithCorrectStatus_When_MultipleSimilarErrors()
    {
        var sut = CreateSut();
        var messages = Fixture.CreateMany<string>(2).ToArray();
        var metadata = Fixture.Create<Dictionary<string, object?>>();
        var errors = Seq<Error>.Empty.Add(new ValidationError(messages[0], metadata)).Add(new ValidationError(messages[1], default!));
        var expectedStatus = (int)_mapping.GetError(errors.First().GetType());

        var details = sut.ConvertToProblem(errors);

        details.Status.Should().Be(expectedStatus);
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
            .WhoseValue.Should().BeEquivalentTo(errorMessage);
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
        using var activity = new Activity("Testing");
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
        _context.TraceIdentifier = id;

        var details = sut.CreateValidationProblemDetails(_context, _state);

        Activity.Current.Should().BeNull();
        details.Should().NotBeNull();
        details.Extensions.Should().ContainKey("traceId")
            .WhoseValue.Should().BeOfType<string>()
            .Which.Should().Be(id);
    }

    private HomeInventoryProblemDetailsFactory CreateSut() => new(_mapping, Options.Create(_options));
}
