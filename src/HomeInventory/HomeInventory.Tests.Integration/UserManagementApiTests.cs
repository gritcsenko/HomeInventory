using System.Net;
using System.Net.Http.Json;
using HomeInventory.Contracts;
using HomeInventory.Domain.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Tests.Integration;

public class UserManagementApiTests : BaseIntegrationTest
{
    private const string _registerRoute = "/api/users/manage/register";
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly JsonContent _content;

    public UserManagementApiTests()
    {
        Fixture.CustomizeRegisterRequest();
        var request = Fixture.Create<RegisterRequest>();
        _content = JsonContent.Create(request);
        AddDisposable(_content);
    }

    [Fact]
    public void VerifyEndpoints()
    {
        Endpoints.Should().ContainEndpoint(_registerRoute, HttpMethods.Post)
            .Which.Metadata.Should().ContainSingle(x => x is AllowAnonymousAttribute);
    }

    [Fact]
    public async Task Register_ReturnsSuccess()
    {
        var response = await PostAsync(_registerRoute, _content);

        response.StatusCode.Should().BeDefined()
            .And.Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>(options: null, Cancellation.Token);
        body.Should().NotBeNull();
        body!.UserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RegisterSameTwice_ReturnsFailure()
    {
        _ = await PostAsync(_registerRoute, _content);
        var response = await PostAsync(_registerRoute, _content);

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
