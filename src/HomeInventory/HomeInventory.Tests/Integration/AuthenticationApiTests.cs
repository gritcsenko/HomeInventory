using System.Net;
using System.Net.Http.Json;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HomeInventory.Tests.Integration;

[IntegrationTest]
public class AuthenticationApiTests : BaseTest, IDisposable
{
    private readonly WebApplicationFactory<Program> _appFactory = new();
    private readonly HttpClient _client;

    public AuthenticationApiTests()
    {
        _client = _appFactory.CreateClient();
        Fixture.Customize(new RegisterRequestCustomization());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _appFactory.Dispose();
        }
        base.Dispose(disposing);
    }

    [Fact]
    public async Task Register_ReturnsSuccess()
    {
        var request = Fixture.Create<RegisterRequest>();
        var content = JsonContent.Create(request);

        var response = await _client.PostAsync("/api/Authentication/register", content, Cancellation.Token);

        response.StatusCode.Should().BeDefined()
            .And.Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>(options: null, Cancellation.Token);
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RegisterSameTwice_ReturnsFailure()
    {
        var request = Fixture.Create<RegisterRequest>();
        var content = JsonContent.Create(request);

        _ = await _client.PostAsync("/api/Authentication/register", content, Cancellation.Token);
        var response = await _client.PostAsync("/api/Authentication/register", content, Cancellation.Token);

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
