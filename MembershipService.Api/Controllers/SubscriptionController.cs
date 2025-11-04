using Microsoft.AspNetCore.Mvc;
using MembershipService.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MembershipService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionService _subscriptionService;

        public SubscriptionController(SubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("subscriptions")]
        public async Task<IActionResult> GetSubscriptions()
        {
            try
            {
                var refIds = new List<string> { "dg-plus-sub-monthly", "dg-plus-sub-yearly" };

                var rawData = await _subscriptionService.GetSubscriptionsWithPricingAsync(refIds);

                if (rawData == null || !rawData.Any())
                {
                    return NotFound(new
                    {
                        subscriptions = new List<object>(),
                        error = new
                        {
                            code = "NOT_FOUND",
                            message = "No subscriptions found."
                        }
                    });
                }

                // Map to the clean format
                var subscriptions = new List<object>();

                foreach (var data in rawData)
                {
                    var refId = data.GetType().GetProperty("RefId")?.GetValue(data)?.ToString() ?? "";
                    var productDetailsJson = data.GetType().GetProperty("ProductDetails")?.GetValue(data);
                    var priceDetailsJson = data.GetType().GetProperty("PriceDetails")?.GetValue(data);

                    if (string.IsNullOrEmpty(refId)) continue;

                    // Determine plan type
                    string planType = refId.Contains("yearly", StringComparison.OrdinalIgnoreCase) ? "YEARLY" : "MONTHLY";
                    string frequency = planType == "YEARLY" ? "12 months" : "1 month";

                    double price = 0.0;
                    bool stockAvailable = true;
                    string status = "INACTIVE";

                    try
                    {
                        if (priceDetailsJson != null)
                        {
                            var priceJson = JsonSerializer.Serialize(priceDetailsJson);
                            using var doc = JsonDocument.Parse(priceJson);
                            var root = doc.RootElement;

                            if (root.TryGetProperty("basePrice", out var basePriceEl) && basePriceEl.ValueKind == JsonValueKind.Number)
                                price = basePriceEl.GetDouble();

                            status = price > 0 ? "ACTIVE" : "INACTIVE";
                        }

                        if (productDetailsJson != null)
                        {
                            var prodJson = JsonSerializer.Serialize(productDetailsJson);
                            using var doc = JsonDocument.Parse(prodJson);
                            var root = doc.RootElement;

                            if (root.TryGetProperty("ShowWithoutStock", out var stockEl) && stockEl.ValueKind == JsonValueKind.True)
                                stockAvailable = true;
                            else
                                stockAvailable = false;
                        }
                    }
                    catch
                    {
                        // ignore parsing errors
                    }

                    // Add transformed data
                    subscriptions.Add(new
                    {
                        planType,
                        frequency,
                        skus = new List<object>
                        {
                            new
                            {
                                skuId = refId,
                                price,
                                status,
                                stockAvailable
                            }
                        }
                    });
                }

                return Ok(new
                {
                    subscriptions,
                    error = (object)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    subscriptions = new List<object>(),
                    error = new
                    {
                        code = "SERVICE_UNAVAILABLE",
                        message = $"Unable to retrieve subscriptions at the moment. Please try later. ({ex.Message})"
                    }
                });
            }
        }
    }
}
