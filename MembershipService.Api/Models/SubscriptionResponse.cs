using System.Reflection;

namespace MembershipService.Api.Models
{
    public class SubscriptionResponse
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public string PlanId { get; set; }
        

    }
}
