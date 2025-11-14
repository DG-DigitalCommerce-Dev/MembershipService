using System.Text.Json.Serialization;

namespace MembershipService.Domain.Models
{
    public class SubscriptionResponse
    {
        [JsonPropertyName("subscriptions")]
        public List<SubscriptionPlan> Subscriptions { get; set; } = new();

        public string? Error { get; set; }
    }
}
