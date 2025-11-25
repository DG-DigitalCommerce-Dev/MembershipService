using System.Text.Json.Serialization;

namespace MembershipService.Domain.Models
{
    public class Subscription
    {
        public List<SubscriptionPlan> Subscriptions { get; set; } = new();
        public string? Error { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("cycleCount")]
        public int CycleCount { get; set; }

        [JsonPropertyName("nextPurchaseDate")]
        public DateTime NextPurchaseDate { get; set; }
        public bool PromotionApplied { get; set; }
    }
}
