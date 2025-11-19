using System.Collections.Generic;
namespace MembershipService.Domain.Models
{
    public class VtexApiSettings
    {
        public string BaseUrl { get; set; }        
        public string PricingBaseUrl { get; set; }  
        public string AppKey { get; set; }
        public string AppToken { get; set; }
        public List<SubscriptionRefSetting> SubscriptionRefs { get; set; } = new();
    }
    public class SubscriptionRefSetting
    {
        public string RefId { get; set; }       
        public string PlanType { get; set; }   
        public string Frequency { get; set; }  
    }
}
