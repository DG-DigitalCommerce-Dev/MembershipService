namespace MembershipService.Api.Models
{
    public class SubscriptionResponseModel
    {
        public string PlanType { get; set; }
        public string Frequency { get; set; }
        public List<SkuResponseModel> Skus { get; set; }
    }

    public class SkuResponseModel
    {
        public string SkuId { get; set; }
        public decimal? Price { get; set; }
        public string Status { get; set; }  
        public bool StockAvailable { get; set; }
    }
}
