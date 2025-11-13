using MembershipService.Domain.Models;
using MembershipService.Infrastructure.Constants;
using MembershipService.Infrastructure.Integrations.Interfaces;
using MembershipService.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;

namespace MembershipService.Infrastructure.Integrations
{
    public class VtexClient : IVtexClient
    {
        private readonly HttpClient _catalogClient;
        private readonly HttpClient _pricingClient;
        private readonly ILogger<VtexClient> _logger;
        private readonly VtexSettings _settings;

        private static readonly Dictionary<string, (string Type, string Frequency)> RefIds = new()
        {
            { "dg-plus-sub-monthly", ("MONTHLY", "1 month") },
            { "dg-plus-sub-yearly", ("YEARLY", "12 months") }
        };

        public VtexClient(HttpClient httpClient, IOptions<VtexSettings> options, ILogger<VtexClient> logger)
        {
            _settings = options.Value;
            _logger = logger;

            _catalogClient = httpClient;
            _catalogClient.BaseAddress = new Uri(_settings.BaseUrl.TrimEnd('/') + "/");
            _catalogClient.DefaultRequestHeaders.Add(VtexConstants.AppKeyHeader, _settings.AppKey);
            _catalogClient.DefaultRequestHeaders.Add(VtexConstants.AppTokenHeader, _settings.AppToken);

            _pricingClient = new HttpClient();
            _pricingClient.BaseAddress = new Uri("https://api.vtex.com/globallogicpartnerus/pricing/");
            _pricingClient.DefaultRequestHeaders.Add(VtexConstants.AppKeyHeader, _settings.AppKey);
            _pricingClient.DefaultRequestHeaders.Add(VtexConstants.AppTokenHeader, _settings.AppToken);
        }

        public async Task<SubscriptionResponse?> GetSubscriptionPlansAsync()
        {
            var plans = new List<SubscriptionPlan>();

            foreach (var entry in RefIds)
            {
                string refId = entry.Key;
                var map = entry.Value;

                var productRoot = await GetWithRetry(() => GetProductAsync(refId), 2);
                if (!productRoot.HasValue)
                    continue;

                var plan = new SubscriptionPlan
                {
                    PlanType = map.Type,
                    Frequency = map.Frequency,
                    Skus = new List<Sku>()
                };

                bool addedSku = false;

                if (productRoot.Value.TryGetProperty("items", out var items) &&
                    items.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        if (item.TryGetProperty("Id", out var idEl))
                        {
                            string skuId = idEl.GetString() ?? "";
                            await AddSku(plan.Skus, skuId, productRoot.Value);
                            addedSku = true;
                        }
                    }
                }

                if (!addedSku &&
                    productRoot.Value.TryGetProperty("Id", out var prodIdEl))
                {
                    string productId = prodIdEl.GetInt32().ToString();
                    await AddSku(plan.Skus, productId, productRoot.Value);
                }

                if (plan.Skus.Count > 0)
                    plans.Add(plan);
            }

            return new SubscriptionResponse
            {
                Subscriptions = plans,
                Error = plans.Count == 0 ? "NOT_FOUND" : null
            };
        }

        private async Task<JsonElement?> GetProductAsync(string refId)
        {
            var resp = await _catalogClient.GetAsync($"api/catalog_system/pvt/products/productgetbyrefid/{refId}");
            if (!resp.IsSuccessStatusCode)
                return null;

            string json = await resp.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json).RootElement;
        }

        private async Task<JsonElement?> GetPriceAsync(string itemId)
        {
            var resp = await _pricingClient.GetAsync($"prices/{itemId}");
            if (!resp.IsSuccessStatusCode)
                return null;

            string json = await resp.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json).RootElement;
        }

        private async Task<T?> GetWithRetry<T>(Func<Task<T?>> func, int retries)
        {
            for (int i = 0; i <= retries; i++)
            {
                var result = await func();
                if (result != null)
                    return result;
                await Task.Delay(300);
            }
            return default;
        }

        private async Task AddSku(List<Sku> list, string itemId, JsonElement productRoot)
        {
            var priceRoot = await GetWithRetry(() => GetPriceAsync(itemId), 2);
            decimal? price = null;

            if (priceRoot.HasValue &&
                priceRoot.Value.TryGetProperty("basePrice", out var priceEl) &&
                priceEl.ValueKind == JsonValueKind.Number)
            {
                price = (decimal)priceEl.GetDouble();
            }

            string status = price.HasValue && price > 0 ? "ACTIVE" : "INACTIVE";

            bool stockAvailable =
                productRoot.TryGetProperty("ShowWithoutStock", out var stockEl) &&
                stockEl.ValueKind == JsonValueKind.True;

            list.Add(new Sku
            {
                SkuId = itemId,
                Price = price,
                Status = status,
                StockAvailable = stockAvailable
            });
        }
    }
}
