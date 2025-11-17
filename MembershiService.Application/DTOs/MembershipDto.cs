using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MembershipService.Domain.Models;

namespace MembershipService.Application.DTOs
{
    public record MembershipResponseDto(
        IEnumerable<MembershipDto> Memberships,
        int TotalCount,
        int PageCount
    );
    public class MembershipDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public string CustomerEmail { get; set; }
        public string Title { get; set; }
        [Required]
        public string Status { get; set; }
        public bool IsSkipped { get; set; }
        public string NextPurchaseDate { get; set; }
        public string LastPurchaseDate { get; set; }
        [Required]
        public PlanDto PlanDto { get; set; }
        public ShippingAddressDto ShippingAddressDto { get; set; }
        public PurchaseSettingsDto PurchaseSettingsDto { get; set; }
        public int CycleCount { get; set; }
        public string CreatedAt { get; set; }
        public string LastUpdate { get; set; }
        public List<ItemDto> ItemDtos { get; set; }
        public string? LastCycleId { get; set; }
        public CustomDataDto CustomData { get; set; }
    }

    public class PlanDto
    {
        public string Id { get; set; }
        public FrequencyDto FrequencyDto { get; set; }
        public ValidityDto ValidityDto { get; set; }
        public string PurchaseDay { get; set; }
    }

    public class FrequencyDto
    {
        public string Periodicity { get; set; }
        public int Interval { get; set; }
    }

    public class ValidityDto
    {
        public string Begin { get; set; }
        public string End { get; set; }
    }

    public class ShippingAddressDto
    {
        public string AddressId { get; set; }
        public string AddressType { get; set; }
    }

    public class PurchaseSettingsDto
    {
        public PaymentMethodDto PaymentMethodDto { get; set; }
        public string CurrencyCode { get; set; }
        public string? SelectedSla { get; set; }
        public string SalesChannel { get; set; }
        public string Seller { get; set; }
    }

    public class PaymentMethodDto
    {
        public string? PaymentAccountId { get; set; }
        public string PaymentSystem { get; set; }
        public int Installments { get; set; }
        public string PaymentSystemName { get; set; }
        public string PaymentSystemGroup { get; set; }
    }

    public class ItemDto
    {
        public string Id { get; set; }
        public string SkuId { get; set; }
        public int Quantity { get; set; }
        public bool IsSkipped { get; set; }
        public string Status { get; set; }
        public int CycleCount { get; set; }
        public decimal PriceAtSubscriptionDate { get; set; }
        public decimal ManualPrice { get; set; } 
        public List<AttachmentDto> AttachmentDtos { get; set; }
        public string OriginalOrderId { get; set; }
    }

    public class AttachmentDto
    {
        public string Name { get; set; }
        public AttachmentContentDto Content { get; set; }
    }

    public class AttachmentContentDto
    {
        public Dictionary<string, object> AdditionalData { get; set; }
    }

    public class CustomDataDto
    {
        public Dictionary<string, object> AdditionalData { get; set; }
    }
}
