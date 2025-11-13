using System.Collections.Generic;
namespace MembershipService.Domain.Models
{
    public class SubscriptionPlan
    {
        public string PlanType { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;

        // NEW: A list of SKUs
        public List<Sku> Skus { get; set; } = new();
    }
}
