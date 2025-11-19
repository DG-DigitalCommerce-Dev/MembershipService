namespace MembershipService.Domain.Models
{
    public class SubscriptionPlan
    {
        public string PlanType { get; set; }
        public string Frequency { get; set; }
        public List<Sku> Skus { get; set; } = new();
    }
}
