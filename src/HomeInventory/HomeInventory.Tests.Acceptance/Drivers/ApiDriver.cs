using Flurl;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal abstract class ApiDriver
{
    private readonly TestServer _server;
    private readonly string _basePath;

    protected ApiDriver(TestServer server, string basePath)
    {
        _server = server;
        _basePath = basePath;
    }

    protected IApiRequestBuilder CreateGetRequest(string path) =>
        CreateRequest(HttpMethod.Get, path);

    protected IApiRequestBuilder CreatePostRequest(string path) =>
        CreateRequest(HttpMethod.Post, path);

    protected IApiRequestBuilder CreateRequest(HttpMethod httpMethod, string path) =>
        new ApiRequestBuilder(_server.CreateRequest(_basePath.AppendPathSegment(path)), httpMethod);
}
