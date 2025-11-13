using System.Text.Json.Serialization;

namespace MembershipService.Domain.Models
{
    public class SubscriptionResponse
    {
        [JsonPropertyName("plans")]   // JSON output name
        public List<SubscriptionPlan> Subscriptions { get; set; } = new();

        public string? Error { get; set; }
    }
}

