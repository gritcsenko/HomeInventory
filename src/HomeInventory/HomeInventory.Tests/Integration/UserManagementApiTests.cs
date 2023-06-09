using System.Net;
using System.Net.Http.Json;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Integration;

[IntegrationTest]
public class UserManagementApiTests : BaseTest
{
#pragma warning disable CA2213 // Disposable fields should be disposed
    private readonly WebApplicationFactory<Program> _appFactory = new();
    private readonly HttpClient _client;
#pragma warning restore CA2213 // Disposable fields should be disposed

    public UserManagementApiTests()
    {
        AddDisposable(_appFactory);

        _client = _appFactory.CreateClient();
        AddDisposable(_client);

        Fixture.Customize(new RegisterRequestCustomization());
    }

    [Fact]
    public void VerifyEndpoints()
    {
        var endpoints = _appFactory.Services
            .GetServices<EndpointDataSource>()
            .SelectMany(s => s.Endpoints)
            .OfType<RouteEndpoint>()
            .ToReadOnly();

        var endpoint = endpoints.Should().ContainSingle(e =>
            e.RoutePattern.RawText == "/api/users/manage/register"
            && e.Metadata.OfType<IHttpMethodMetadata>().First().HttpMethods.Contains(HttpMethods.Post))
            .Subject;
        endpoint.Metadata.Should().ContainSingle(x => x is AllowAnonymousAttribute);
    }

    [Fact]
    public async Task Register_ReturnsSuccess()
    {
        var request = Fixture.Create<RegisterRequest>();
        using var content = JsonContent.Create(request);

        var response = await _client.PostAsync(new Uri("/api/users/manage/register", UriKind.Relative), content, Cancellation.Token);

        response.StatusCode.Should().BeDefined()
            .And.Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>(options: null, Cancellation.Token);
        body.Should().NotBeNull();
        body!.UserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RegisterSameTwice_ReturnsFailure()
    {
        var request = Fixture.Create<RegisterRequest>();
        using var content = JsonContent.Create(request);

        _ = await _client.PostAsync(new Uri("/api/users/manage/register", UriKind.Relative), content, Cancellation.Token);
        var response = await _client.PostAsync(new Uri("/api/users/manage/register", UriKind.Relative), content, Cancellation.Token);

        response.StatusCode.Should().BeDefined()
            .And.Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(options: null, Cancellation.Token)!;
        body!.Should().NotBeNull();
        body!.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.8");
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
