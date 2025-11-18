using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MembershipService.Api.Models
{
    public class PaginatedMembershipResponse
    {
        public PaginatedMembershipResponse(IEnumerable<MembershipResponse> memberships, int totalCount)
        {
            MembershipList = memberships;
            TotalCount = totalCount;
        }

        public IEnumerable<MembershipResponse> MembershipList { get; set; }
        public int TotalCount { get; set; }
    }
    public class MembershipResponse
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public bool IsSkipped { get; set; }
        public string NextPurchaseDate { get; set; }
        public string LastPurchaseDate { get; set; }
        public Plan Plan { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public PurchaseSettings PurchaseSettings { get; set; }
        public int CycleCount { get; set; }
        public string CreatedAt { get; set; }
        public string LastUpdate { get; set; }
        public List<Item> Items { get; set; }
        public string? LastCycleId { get; set; }
        public CustomData CustomData { get; set; }
    }

    public class Plan
    {
        public string Id { get; set; }
        public Frequency Frequency { get; set; }
        public Validity Validity { get; set; }
        public string PurchaseDay { get; set; }
    }

    public class Frequency
    {
        public string Periodicity { get; set; }
        public int Interval { get; set; }
    }

    public class Validity
    {
        public string Begin { get; set; }
        public string End { get; set; }
    }

    public class ShippingAddress
    {
        public string AddressId { get; set; }
        public string AddressType { get; set; }
    }

    public class PurchaseSettings
    {
        public PaymentMethod PaymentMethod { get; set; }
        public string CurrencyCode { get; set; }
        public string? SelectedSla { get; set; }
        public string SalesChannel { get; set; }
        public string Seller { get; set; }
    }

    public class PaymentMethod
    {
        public string PaymentAccountId { get; set; }
        public string PaymentSystem { get; set; }
        public int Installments { get; set; }
        public string PaymentSystemName { get; set; }
        public string PaymentSystemGroup { get; set; }
    }

    public class Item
    {
        public string Id { get; set; }
        public string SkuId { get; set; }
        public int Quantity { get; set; }
        public bool IsSkipped { get; set; }
        public string Status { get; set; }
        public int CycleCount { get; set; }
        public decimal PriceAtSubscriptionDate { get; set; }
        public decimal ManualPrice { get; set; }
        public List<Attachment> Attachments { get; set; }
        public string OriginalOrderId { get; set; }
    }

    public class Attachment
    {
        public string Name { get; set; }
        public AttachmentContent Content { get; set; }
    }

    public class AttachmentContent
    {
        public Dictionary<string, object> AdditionalData { get; set; }
    }

    public class CustomData
    {
        public Dictionary<string, object> AdditionalData { get; set; }
    }
}
