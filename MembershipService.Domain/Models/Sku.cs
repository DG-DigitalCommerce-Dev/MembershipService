namespace MembershipService.Domain.Models
{
    public class Sku
    {
        public string SkuId { get; set; }
        public decimal? Price { get; set; }
        public string Status { get; set; }
        public bool IsStockAvailable { get; set; }
    }
}
