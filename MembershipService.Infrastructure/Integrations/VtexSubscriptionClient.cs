using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Constants;
using MembershipService.Infrastructure.Interfaces;
using MembershipService.Domain.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
namespace MembershipService.Infrastructure.Integrations
{
    public class VtexSubscriptionClient : IVtexSubscriptionClient
    {
        private readonly HttpClient _catalogClient;
        private readonly HttpClient _pricingClient;
        private readonly ILogger<VtexSubscriptionClient> _logger;
        private readonly VtexApiSettings _settings;
        public VtexSubscriptionClient(
            HttpClient httpClient,
            IOptions<VtexApiSettings> options,
            ILogger<VtexSubscriptionClient> logger)
        {
            _settings = options.Value;
            _logger = logger;
            _catalogClient = httpClient;
            _catalogClient.BaseAddress = new Uri($"{_settings.BaseUrl.TrimEnd('/')}/");
            ApplyDefaultHeaders(_catalogClient);
            _pricingClient = new HttpClient();
            _pricingClient.BaseAddress = new Uri($"{_settings.PricingBaseUrl.TrimEnd('/')}/");
            ApplyDefaultHeaders(_pricingClient);
        }
        private void ApplyDefaultHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Add(VtexConstants.AppKeyHeader, _settings.AppKey);
            client.DefaultRequestHeaders.Add(VtexConstants.AppTokenHeader, _settings.AppToken);
            client.DefaultRequestHeaders.Add(VtexConstants.AcceptHeader, VtexConstants.AcceptHeaderValue);
        }
        public async Task<SubscriptionResponse?> GetSubscriptionPlansAsync()
        {
            try
            {
                var plans = new List<SubscriptionPlan>();
                foreach (var refItem in _settings.SubscriptionRefs)
                {
                    _logger.LogInformation(LogMessages.FetchProduct, refItem.RefId);
                    var productJson = await FetchProduct(refItem.RefId);
                    if (productJson is null) continue;
                    var skuIds = ExtractSkuIds(productJson.Value);
                    var plan = new SubscriptionPlan
                    {
                        PlanType = refItem.PlanType,
                        Frequency = refItem.Frequency
                    };
                    foreach (var skuId in skuIds)
                    {
                        _logger.LogInformation(LogMessages.FetchPrice, skuId);
                        var sku = await BuildSku(skuId, productJson.Value);
                        if (sku != null)
                            plan.Skus.Add(sku);
                    }
                    if (plan.Skus.Count > 0)
                        plans.Add(plan);
                }
                return new SubscriptionResponse { Subscriptions = plans };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LogMessages.VtexFetchError);
                return new SubscriptionResponse
                {
                    Subscriptions = new List<SubscriptionPlan>(),
                    Error = ex.Message
                };
            }
        }
        private async Task<JsonElement?> FetchProduct(string refId)
        {
            try
            {
                var response = await _catalogClient.GetAsync(
                    $"api/catalog_system/pvt/products/productgetbyrefid/{refId}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonDocument.Parse(json).RootElement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LogMessages.ProductFetchError, refId);
                return null;
            }
        }
        private List<string> ExtractSkuIds(JsonElement product)
        {
            var skuIds = new List<string>();
            if (product.TryGetProperty("items", out var items) &&
                items.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in items.EnumerateArray())
                {
                    if (item.TryGetProperty("Id", out var idProp))
                        skuIds.Add(idProp.GetString() ?? string.Empty);
                }
            }
            if (!skuIds.Any() &&
                product.TryGetProperty("Id", out var fallbackId))
            {
                skuIds.Add(fallbackId.GetInt32().ToString());
            }
            return skuIds;
        }
        private async Task<Sku?> BuildSku(string skuId, JsonElement productJson)
        {
            try
            {
                var priceJson = await FetchSkuPrice(skuId);
                decimal? price = null;
                if (priceJson is JsonElement priceElement &&
                    priceElement.TryGetProperty("basePrice", out var basePriceProp) &&
                    basePriceProp.ValueKind == JsonValueKind.Number)
                {
                    price = basePriceProp.GetDecimal();
                }
                var stockAvailable =
                    productJson.TryGetProperty("ShowWithoutStock", out var stockProp) &&
                    stockProp.ValueKind == JsonValueKind.True;
                var status = price.HasValue && price.Value > 0 ? "ACTIVE" : "INACTIVE";
                return new Sku
                {
                    SkuId = skuId,
                    Price = price,
                    Status = status,
                    StockAvailable = stockAvailable
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LogMessages.SkuBuildError, skuId);
                return null;
            }
        }
        private async Task<JsonElement?> FetchSkuPrice(string skuId)
        {
            try
            {
                var response = await _pricingClient.GetAsync($"prices/{skuId}");

                if (!response.IsSuccessStatusCode)
                {
                    await Task.Delay(200);
                    response = await _pricingClient.GetAsync($"prices/{skuId}");
                    if (!response.IsSuccessStatusCode) return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                return JsonDocument.Parse(json).RootElement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LogMessages.PriceFetchError, skuId);
                return null;
            }
        }

    }
}

