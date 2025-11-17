using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
namespace MembershipService.Domain.Models
{
    public class VtexMembershipResponse
    {
        public VtexMembershipResponse(IEnumerable<MembershipData> memberships, int totalCount, int pageCount)
        {
            Memberships = memberships;
            TotalCount = totalCount;
            PageCount = pageCount;
        }

        public IEnumerable<MembershipData> Memberships { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
    }
    public class MembershipData
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public bool IsSkipped { get; set; }
        public string NextPurchaseDate { get; set; }
        public string LastPurchaseDate { get; set; }
        public PlanData Plan { get; set; }
        public ShippingAddressData ShippingAddress { get; set; }
        public PurchaseSettingsData PurchaseSettings { get; set; }
        public int CycleCount { get; set; }
        public string CreatedAt { get; set; }
        public string LastUpdate { get; set; }
        public List<ItemData> Items { get; set; }
        public string? LastCycleId { get; set; }
        public CustomDataObject CustomData { get; set; }
    }

    public class PlanData
    {
        public string Id { get; set; }
        public FrequencyData Frequency { get; set; }
        public ValidityData Validity { get; set; }
        public string PurchaseDay { get; set; } 
    }

    public class FrequencyData
    {
        public string Periodicity { get; set; }
        public int Interval { get; set; }
    }

    public class ValidityData
    {
        public string Begin { get; set; }
        public string End { get; set; }
    }

    public class ShippingAddressData
    {
        public string AddressId { get; set; }
        public string AddressType { get; set; }
    }

    public class PurchaseSettingsData
    {
        public PaymentMethodData PaymentMethod { get; set; }
        public string CurrencyCode { get; set; }
        public string? SelectedSla { get; set; }
        public string SalesChannel { get; set; }
        public string Seller { get; set; }
    }

    public class PaymentMethodData
    {
        public string? PaymentAccountId { get; set; }
        public string PaymentSystem { get; set; }
        public int Installments { get; set; }
        public string PaymentSystemName { get; set; }
        public string PaymentSystemGroup { get; set; }
    }

    public class ItemData
    {
        public string Id { get; set; }
        public string SkuId { get; set; }
        public int Quantity { get; set; }
        public bool IsSkipped { get; set; }
        public string Status { get; set; }
        public int CycleCount { get; set; }
        public decimal PriceAtSubscriptionDate { get; set; } 
        public decimal ManualPrice { get; set; } 
        public List<AttachmentData> Attachments { get; set; }
        public string OriginalOrderId { get; set; }
    }

    public class AttachmentData
    {
        public string Name { get; set; }
        public AttachmentContentData Content { get; set; }
    }

    public class AttachmentContentData
    {
        public Dictionary<string, object> AdditionalData { get; set; }
    }

    public class CustomDataObject
    {
        public Dictionary<string, object> AdditionalData { get; set; }
    }
}
