namespace HomeInventory.Tests.Acceptance.Drivers;

internal interface IApiRequestBuilder
{
    IApiRequestBuilder WithRequestHeader(string name, string value);

    IApiRequestBuilder WithBody(HttpContent content);

    Task<HttpResponseMessage> SendAsync();
}
