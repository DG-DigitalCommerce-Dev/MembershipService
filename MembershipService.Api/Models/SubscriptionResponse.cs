namespace MembershipService.Api.Models
{
    public class SubscriptionResponse
    {
        public string PlanType { get; set; }
        public string Frequency { get; set; }
        public List<SkuResponse> Skus { get; set; }
    }
    public class SkuResponse
    {
        public string SkuId { get; set; }
        public decimal? Price { get; set; }
        public string Status { get; set; }
        public bool StockAvailable { get; set; }
    }
}
