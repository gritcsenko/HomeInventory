using System.Net;
using System.Net.Http.Json;
using FluentAssertions.Execution;
using Flurl;
using HomeInventory.Contracts;
using HomeInventory.Core;
using HomeInventory.Domain.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace HomeInventory.Tests.Integration;

public class UserManagementApiTests : BaseIntegrationTest
{
    private static readonly string _registerRoute = "/".AppendPathSegments("api", "users", "manage", "register");
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly JsonContent _content;

    public UserManagementApiTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
        Fixture.CustomizeRegisterRequest();
        var request = Fixture.Create<RegisterRequest>();
        _content = JsonContent.Create(request);
        AddDisposable(_content);
    }

    [Fact]
    [TestPriority(1)]
    public void VerifyEndpoints()
    {
        using var scope = new AssertionScope();
        var endpoints = GetEndpoints().ToReadOnly();
        endpoints.Should().ContainEndpoint(_registerRoute, HttpMethods.Post)
            .Which.Metadata.Should().ContainSingle(x => x is AllowAnonymousAttribute);
    }

    [Fact]
    [TestPriority(2)]
    public async Task Register_ReturnsSuccess()
    {
        var response = await PostAsync(_registerRoute, _content);

        using var scope = new AssertionScope();
        response.StatusCode.Should().BeDefined()
            .And.Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>(options: null, Cancellation.Token);
        body.Should().NotBeNull();
        body!.UserId.Should().NotBeEmpty();
    }

    [Fact]
    [TestPriority(3)]
    public async Task RegisterSameTwice_ReturnsFailure()
    {
        _ = await PostAsync(_registerRoute, _content);
        var response = await PostAsync(_registerRoute, _content);

        using var scope = new AssertionScope();
        response.StatusCode.Should().BeDefined()
            .And.Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(options: null, Cancellation.Token)!;
        body!.Should().NotBeNull();
        body!.Type.Should().Be("https://tools.ietf.org/html/rfc9110#section-15.5.1");
        body!.Status.Should().Be(StatusCodes.Status409Conflict);
        body!.Title.Should().Be(nameof(DuplicateEmailError));
        body!.Detail.Should().Be(DuplicateEmailError.DefaultMessage);
        body!.Instance.Should().BeNull();
        body!.Extensions.Should().ContainKey("traceId")
            .WhoseValue.Should().BeJsonElement()
            .Which.GetString().Should().NotBeNullOrEmpty();
        body!.Extensions.Should().ContainKey("errorCodes")
            .WhoseValue.Should().BeJsonElement()
            .Which.Should().BeArrayEqualTo(new[] { nameof(DuplicateEmailError) });
    }
}
