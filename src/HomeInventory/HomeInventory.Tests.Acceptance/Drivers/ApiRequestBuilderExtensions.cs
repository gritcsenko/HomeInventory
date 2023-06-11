using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Throw;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal static class ApiRequestBuilderExtensions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Content will be disposed")]
    public static IApiRequestBuilder WithJsonBody<TRequest>(this IApiRequestBuilder builder, TRequest requestBody, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null) =>
        builder.WithBody(JsonContent.Create(requestBody, mediaType, options));

    public static async ValueTask<HttpResponseMessage> SendAndEnsureSuccessStatusCodeAsync(this IApiRequestBuilder builder) =>
        await builder.SendAsync(r => r.EnsureSuccessStatusCodeAsync());

    public static async ValueTask<TResponse> SendAsync<TResponse>(this IApiRequestBuilder builder)
        where TResponse : class
    {
        var result = await builder.SendAndEnsureSuccessStatusCodeAsync();
        var body = await result.Content.ReadFromJsonAsync<TResponse>();
        return body.ThrowIfNull().Value;
    }

    public static async ValueTask<HttpResponseMessage> SendAsync(this IApiRequestBuilder builder, Func<HttpResponseMessage, ValueTask> handleAsync)
    {
        var result = await builder.SendAsync();
        await handleAsync(result);
        return result;
    }

    private static async ValueTask EnsureSuccessStatusCodeAsync(this HttpResponseMessage result)
    {
        if (!result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Response status code does not indicate success: {(int)result.StatusCode} ({result.ReasonPhrase}). Content: {content}");
        }
    }
}
