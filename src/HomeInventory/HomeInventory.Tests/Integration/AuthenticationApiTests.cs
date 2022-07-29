using AutoFixture;
using FluentAssertions;
using HomeInventory.Contracts;
using HomeInventory.Tests.Helpers;
using HomeInventory.Tests.Systems.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace HomeInventory.Tests.Integration;
[Trait("Category", "Integration")]
public class AuthenticationApiTests : BaseTest, IAsyncDisposable
{
    private readonly WebApplicationFactory<Program> _appFactory;
    private readonly HttpClient _client;

    public AuthenticationApiTests()
    {
        _appFactory = new WebApplicationFactory<Program>();
        _client = _appFactory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        await _appFactory.DisposeAsync();
    }

    [Fact]
    public async Task Register_ReturnsSuccess()
    {
        var request = Fixture.Create<RegisterRequest>();
        var content = JsonContent.Create(request);

        var response = await _client.PostAsync("/api/Authentication/register", content, CancellationToken);

        response.StatusCode.Should().BeDefined()
            .And.Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>(options: null, CancellationToken);
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RegisterSameTwice_ReturnsFailure()
    {
        var request = Fixture.Create<RegisterRequest>();
        var content = JsonContent.Create(request);

        _ = await _client.PostAsync("/api/Authentication/register", content, CancellationToken);
        var response = await _client.PostAsync("/api/Authentication/register", content, CancellationToken);

        response.StatusCode.Should().BeDefined()
            .And.Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(options: null, CancellationToken)!;
        body!.Should().NotBeNull();
        body!.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.8");
        body!.Status.Should().Be(StatusCodes.Status409Conflict);
        body!.Title.Should().Be("User.DuplicateEmail");
        body!.Detail.Should().Be("Duplicate email");
        body!.Instance.Should().BeNull();
        body!.Extensions.Should().ContainKey("errorCodes")
            .WhoseValue.Should().BeJsonElement()
            .Which.Should().BeArrayEqualTo(new[] { "User.DuplicateEmail" });
        body!.Extensions.Should().ContainKey("traceId")
            .WhoseValue.Should().BeJsonElement()
            .Which.GetString().Should().NotBeNullOrEmpty();
    }
}
