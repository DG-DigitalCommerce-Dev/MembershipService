namespace MembershipService.Domain.Models
{
    public class Subscription
    {
        public List<SubscriptionPlan> Subscriptions { get; set; } = new();
        public string? Error { get; set; }
    }
}
