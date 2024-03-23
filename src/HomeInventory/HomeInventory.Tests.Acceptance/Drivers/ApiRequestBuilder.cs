using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class ApiRequestBuilder(RequestBuilder requestBuilder, HttpMethod httpMethod) : IApiRequestBuilder
{
    private readonly HttpMethod _httpMethod = httpMethod;
    private readonly RequestBuilder _requestBuilder = requestBuilder;

    public IApiRequestBuilder WithRequestHeader(string name, string value)
    {
        _requestBuilder.AddHeader(name, value);
        return this;
    }

    public IApiRequestBuilder WithBody(HttpContent content)
    {
        _requestBuilder.And(m => m.Content = content);
        return this;
    }

    public async Task<HttpResponseMessage> SendAsync()
    {
        return await _requestBuilder.SendAsync(_httpMethod.Method);
    }
}
