using System.Net;
using System.Net.Http.Json;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HomeInventory.Tests.Integration;

[IntegrationTest]
public class AuthenticationApiTests : BaseTest, IDisposable
{
    private readonly WebApplicationFactory<Program> _appFactory;
    private readonly HttpClient _client;

    public AuthenticationApiTests()
    {
        _appFactory = new WebApplicationFactory<Program>();
        _client = _appFactory.CreateClient();
    }

    public void Dispose()
    {
        _appFactory.Dispose();
        GC.SuppressFinalize(this);
    }

    [BrokenTest]
    [Fact(Skip = "No reason")]
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

    [BrokenTest]
    [Fact(Skip = "No reason")]
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
