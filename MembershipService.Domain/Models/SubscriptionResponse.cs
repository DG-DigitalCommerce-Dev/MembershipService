namespace MembershipService.Domain.Models
{
    public class SubscriptionResponse
    {
        public List<SubscriptionPlan> Subscriptions { get; set; } = new();
        public string? Error { get; set; }
    }
}
