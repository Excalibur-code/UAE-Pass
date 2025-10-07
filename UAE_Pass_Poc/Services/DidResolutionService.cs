using Newtonsoft.Json;
using UAE_Pass_Poc.Models;

namespace UAE_Pass_Poc.Services
{
    public class DidResolutionService : IDidResolutionService
    {
        private readonly ILogger<DidResolutionService> _logger;
        private readonly HttpClient _httpClient;
        // Configuration for the UAEPASS DID Resolver endpoint
        private readonly string _didResolverEndpoint;

        public DidResolutionService(ILogger<DidResolutionService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            // This endpoint should be configured securely (e.g., from appsettings.json, environment variables)
            // Example: "https://did.uaepass.ae/resolver/" or similar
            _didResolverEndpoint = Environment.GetEnvironmentVariable("UAEPASS_DID_RESOLVER_ENDPOINT")
                                   ?? "https://example.uaepass.did.resolver.com/"; // Placeholder
        }

        public async Task<DidDocument?> ResolveDid(string did)
        {
            if (string.IsNullOrEmpty(did))
            {
                _logger.LogWarning("Attempted to resolve a null or empty DID.");
                return null;
            }

            _logger.LogInformation($"Attempting to resolve DID: {did}");

            try
            {
                // Construct the URL for the DID resolution request
                // This format is common: {resolver_endpoint}/{did}
                string requestUrl = $"{_didResolverEndpoint}{did}";

                // Make an HTTP GET request to the DID resolver endpoint
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode(); // Throws an exception for 4xx or 5xx responses

                string jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogDebug($"DID resolution response for {did}: {jsonResponse}");

                // Deserialize the JSON response into your DidDocument model
                var didDocument = JsonConvert.DeserializeObject<DidDocument>(jsonResponse);

                if (didDocument == null)
                {
                    _logger.LogWarning($"Failed to deserialize DID Document for DID: {did}. Response: {jsonResponse}");
                    return null;
                }

                _logger.LogInformation($"Successfully resolved DID: {did}");
                return didDocument;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP request failed during DID resolution for DID: {did}.");
                // Handle specific HTTP errors (e.g., 404 Not Found for unknown DIDs)
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Failed to deserialize DID Document JSON for DID: {did}.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred during DID resolution for DID: {did}.");
                return null;
            }
        }
    }
}