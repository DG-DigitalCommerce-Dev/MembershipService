namespace MembershipService.Application.DTOs
{
    public class SubscriptionDto
    {
        public string PlanType { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public List<SkuDto> Skus { get; set; } = new();
    }
    public class SkuDto
    {
        public string SkuId { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool StockAvailable { get; set; }
    }
}
