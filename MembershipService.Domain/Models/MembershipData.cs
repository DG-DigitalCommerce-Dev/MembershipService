using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace MembershipService.Domain.Models
{
    public class MembershipData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; }

        [JsonPropertyName("customerEmail")]
        public string CustomerEmail { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("isSkipped")]
        public bool IsSkipped { get; set; }

        [JsonPropertyName("nextPurchaseDate")]
        public string NextPurchaseDate { get; set; }

        [JsonPropertyName("lastPurchaseDate")]
        public string LastPurchaseDate { get; set; }

        [JsonPropertyName("plan")]
        public PlanData Plan { get; set; }

        [JsonPropertyName("shippingAddress")]
        public ShippingAddressData ShippingAddress { get; set; }

        [JsonPropertyName("purchaseSettings")]
        public PurchaseSettingsData PurchaseSettings { get; set; }

        [JsonPropertyName("cycleCount")]
        public int CycleCount { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("lastUpdate")]
        public string LastUpdate { get; set; }

        [JsonPropertyName("items")]
        public List<ItemData> Items { get; set; }

        [JsonPropertyName("lastCycleId")]
        public string? LastCycleId { get; set; }

        [JsonPropertyName("customData")]
        public CustomDataObject CustomData { get; set; }
    }

    public class PlanData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("frequency")]
        public FrequencyData Frequency { get; set; }

        [JsonPropertyName("validity")]
        public ValidityData Validity { get; set; }

        [JsonPropertyName("purchaseDay")]
        public string PurchaseDay { get; set; } 
    }

    public class FrequencyData
    {
        [JsonPropertyName("periodicity")]
        public string Periodicity { get; set; }

        [JsonPropertyName("interval")]
        public int Interval { get; set; }
    }

    public class ValidityData
    {
        [JsonPropertyName("begin")]
        public string Begin { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }
    }

    public class ShippingAddressData
    {
        [JsonPropertyName("addressId")]
        public string AddressId { get; set; }

        [JsonPropertyName("addressType")]
        public string AddressType { get; set; }
    }

    public class PurchaseSettingsData
    {
        [JsonPropertyName("paymentMethod")]
        public PaymentMethodData PaymentMethod { get; set; }

        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("selectedSla")]
        public string? SelectedSla { get; set; }

        [JsonPropertyName("salesChannel")]
        public string SalesChannel { get; set; }

        [JsonPropertyName("seller")]
        public string Seller { get; set; }
    }

    public class PaymentMethodData
    {
        [JsonPropertyName("paymentAccountId")]
        public string? PaymentAccountId { get; set; }

        [JsonPropertyName("paymentSystem")]
        public string PaymentSystem { get; set; }

        [JsonPropertyName("installments")]
        public int Installments { get; set; }

        [JsonPropertyName("paymentSystemName")]
        public string PaymentSystemName { get; set; }

        [JsonPropertyName("paymentSystemGroup")]
        public string PaymentSystemGroup { get; set; }
    }

    public class ItemData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("skuId")]
        public string SkuId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("isSkipped")]
        public bool IsSkipped { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("cycleCount")]
        public int CycleCount { get; set; }

        [JsonPropertyName("priceAtSubscriptionDate")]
        public decimal PriceAtSubscriptionDate { get; set; } // Use decimal for money

        [JsonPropertyName("manualPrice")]
        public decimal ManualPrice { get; set; } // Use decimal for money

        [JsonPropertyName("attachments")]
        public List<AttachmentData> Attachments { get; set; }

        [JsonPropertyName("originalOrderId")]
        public string OriginalOrderId { get; set; }
    }

    public class AttachmentData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("content")]
        public AttachmentContentData Content { get; set; }
    }

    public class AttachmentContentData
    {

        [JsonExtensionData]
        public Dictionary<string, object> AdditionalData { get; set; }
    }

    public class CustomDataObject
    {
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalData { get; set; }
    }
}
