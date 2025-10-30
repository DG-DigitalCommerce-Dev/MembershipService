namespace MembershipService.Domain.Models
{
    public class Sku
    {
        public string SkuId { get; set; } = string.Empty;
        public string Ean { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool StockAvailable { get; set; }
    }
}
