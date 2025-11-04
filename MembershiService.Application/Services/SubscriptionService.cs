using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

namespace MembershipService.Application.Services
{
    public class SubscriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _pricingClient;
        private readonly string _appKey;
        private readonly string _appToken;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(HttpClient httpClient, IConfiguration configuration, ILogger<SubscriptionService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var vtexConfig = configuration.GetSection("VtexApi");
            var baseUrl = vtexConfig["BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("VtexApi:BaseUrl not configured in appsettings.json");

            _httpClient.BaseAddress = new Uri(baseUrl);
            _appKey = vtexConfig["AppKey"];
            _appToken = vtexConfig["AppToken"];

            _httpClient.DefaultRequestHeaders.Add("X-VTEX-API-AppKey", _appKey);
            _httpClient.DefaultRequestHeaders.Add("X-VTEX-API-AppToken", _appToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _pricingClient = new HttpClient(
                new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                })
            {
                BaseAddress = new Uri("https://api.vtex.com/globallogicpartnerus/pricing/")
            };
            _pricingClient.DefaultRequestHeaders.Add("X-VTEX-API-AppKey", _appKey);
            _pricingClient.DefaultRequestHeaders.Add("X-VTEX-API-AppToken", _appToken);
            _pricingClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        
        private async Task<HttpResponseMessage> RetryRequestAsync(Func<Task<HttpResponseMessage>> requestFunc, int maxRetries = 2)
        {
            int delay = 500; // ms
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var response = await requestFunc();
                    if (response.IsSuccessStatusCode)
                        return response;

                    _logger.LogWarning("Request failed with status {StatusCode}. Attempt {Attempt}/{MaxRetries}", response.StatusCode, attempt + 1, maxRetries + 1);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Request attempt {Attempt}/{MaxRetries} failed due to {Message}", attempt + 1, maxRetries + 1, ex.Message);
                }

                if (attempt < maxRetries)
                {
                    await Task.Delay(delay);
                    delay *= 2; // exponential backoff
                }
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable);
        }

        public async Task<List<object>> GetSubscriptionsWithPricingAsync(List<string> refIds)
        {
            var results = new List<object>();

            if (refIds == null || refIds.Count == 0)
            {
                _logger.LogWarning("No refIds provided to GetSubscriptionsWithPricingAsync");
                return results;
            }

            foreach (var refId in refIds)
            {
                try
                {
                    _logger.LogInformation("Fetching product for RefId: {RefId}", refId);

                    // Step 1: Get Product
                    var productResponse = await RetryRequestAsync(() => _httpClient.GetAsync($"api/catalog_system/pvt/products/productgetbyrefid/{refId}"));
                    if (!productResponse.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("VTEX API failed for RefId {RefId} with status {StatusCode}", refId, productResponse.StatusCode);
                        results.Add(new
                        {
                            RefId = refId,
                            ProductDetails = (object)null,
                            PriceDetails = (object)null,
                            Error = "VTEX API unavailable"
                        });
                        continue;
                    }

                    var productJson = await productResponse.Content.ReadAsStringAsync();
                    using var jsonDoc = JsonDocument.Parse(productJson);
                    var root = jsonDoc.RootElement;

                    bool priceFound = false;

                    // Step 2: Extract SKUs
                    if (root.TryGetProperty("items", out JsonElement items) && items.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in items.EnumerateArray())
                        {
                            if (item.TryGetProperty("Id", out JsonElement idElement))
                            {
                                var skuId = idElement.GetString();
                                _logger.LogInformation("Fetching pricing for SKU {SkuId}", skuId);

                                var priceResponse = await RetryRequestAsync(() => _pricingClient.GetAsync($"prices/{skuId}"));
                                if (priceResponse.IsSuccessStatusCode)
                                {
                                    var priceJson = await priceResponse.Content.ReadAsStringAsync();
                                    results.Add(new
                                    {
                                        RefId = refId,
                                        ItemId = skuId,
                                        ProductDetails = JsonSerializer.Deserialize<object>(productJson),
                                        PriceDetails = JsonSerializer.Deserialize<object>(priceJson)
                                    });
                                    priceFound = true;
                                }
                                else
                                {
                                    _logger.LogError("Failed to fetch price for SKU {SkuId}. Status: {Status}", skuId, priceResponse.StatusCode);
                                    results.Add(new
                                    {
                                        RefId = refId,
                                        ItemId = skuId,
                                        ProductDetails = JsonSerializer.Deserialize<object>(productJson),
                                        PriceDetails = "Error fetching pricing"
                                    });
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Invalid SKU in product for RefId {RefId}", refId);
                            }
                        }
                    }

                    // Step 3: Fallback to ProductId if no SKU
                    if (!priceFound && root.TryGetProperty("Id", out JsonElement productIdElem))
                    {
                        var productId = productIdElem.GetInt32();
                        _logger.LogInformation("No SKU found, fetching pricing using ProductId {ProductId}", productId);

                        var priceResponse = await RetryRequestAsync(() => _pricingClient.GetAsync($"prices/{productId}"));
                        if (priceResponse.IsSuccessStatusCode)
                        {
                            var priceJson = await priceResponse.Content.ReadAsStringAsync();
                            results.Add(new
                            {
                                RefId = refId,
                                ProductId = productId,
                                ProductDetails = JsonSerializer.Deserialize<object>(productJson),
                                PriceDetails = JsonSerializer.Deserialize<object>(priceJson)
                            });
                        }
                        else
                        {
                            _logger.LogError("Failed to fetch price for ProductId {ProductId}. Status: {Status}", productId, priceResponse.StatusCode);
                            results.Add(new
                            {
                                RefId = refId,
                                ProductId = productId,
                                ProductDetails = JsonSerializer.Deserialize<object>(productJson),
                                PriceDetails = "Error fetching pricing"
                            });
                        }
                    }

                    // Step 4: No pricing or SKU found
                    if (!priceFound && !root.TryGetProperty("Id", out _))
                    {
                        _logger.LogWarning("No SKU or ProductId found for RefId {RefId}", refId);
                        results.Add(new
                        {
                            RefId = refId,
                            ProductDetails = JsonSerializer.Deserialize<object>(productJson),
                            PriceDetails = "No SKU or Product ID found for pricing"
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error occurred for RefId {RefId}", refId);
                    results.Add(new
                    {
                        RefId = refId,
                        Error = $"Error fetching product: {ex.Message}"
                    });
                }
            }

            if (results.Count == 0)
            {
                _logger.LogWarning("No subscriptions found for provided refIds");
            }

            return results;
        }
    }
}
