namespace HomeInventory.UI.Services.Endpoints;

internal class DebugHttpHandler : DelegatingHandler
{
    private readonly ILogger _logger;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by base class")]
    public DebugHttpHandler(ILogger<DebugHttpHandler> logger, HttpMessageHandler? innerHandler = null)
        : base(innerHandler ?? new HttpClientHandler())
    {
        _logger = logger;
    }

    protected async override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
#if DEBUG
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogDebugMessage("Unsuccessful API Call");
            if (request.RequestUri is not null)
            {
                _logger.LogDebugMessage($"{request.RequestUri} ({request.Method})");
            }
            
            foreach ((var key, var values) in request.Headers.ToDictionary(x => x.Key, x => string.Join(", ", x.Value)))
            {
                _logger.LogDebugMessage($"{key}: {values}");
            }

            var content = request.Content is not null ? await request.Content.ReadAsStringAsync(cancellationToken) : null;
            if (!string.IsNullOrEmpty(content))
            {
                _logger.LogDebugMessage(content);
            }

            // Uncomment to automatically break when an API call fails while debugging
            //// System.Diagnostics.Debugger.Break();
        }
#endif
        return response;
    }
}
