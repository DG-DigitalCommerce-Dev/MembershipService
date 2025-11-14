using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MembershipService.Domain.Models;

namespace MembershipService.Application.DTOs
{
    public class MembershipDto
    {
        public MembershipDto()
        {
        }

        public MembershipDto(MembershipInfo source)
        {
            (Id, CustomerId, CustomerEmail, Title, Status, IsSkipped, NextPurchaseDate, LastPurchaseDate, CycleCount, CreatedAt, LastUpdate, LastCycleId) = (source.Id, source.CustomerId, source.CustomerEmail, source.Title, source.Status, source.IsSkipped, source.NextPurchaseDate, source.LastPurchaseDate, source.CycleCount, source.CreatedAt, source.LastUpdate, source.LastCycleId);
            PlanDto = source.Plan is null ? null : new PlanDto(source.Plan);
            ShippingAddressDto = source.ShippingAddress is null ? null : new ShippingAddressDto(source.ShippingAddress);
            PurchaseSettingsDto = source.PurchaseSettings is null ? null : new PurchaseSettingsDto(source.PurchaseSettings);
            ItemDtos = source.Items?.ConvertAll(item => new ItemDto(item));
            CustomData = source.CustomData is null ? null : new CustomDataDto(source.CustomData);
        }
        public string Id { get; set; }

        public string CustomerId { get; set; }

        public string CustomerEmail { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public bool IsSkipped { get; set; }

        public string NextPurchaseDate { get; set; }

        public string LastPurchaseDate { get; set; }

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
        public PlanDto()
        {
        }

        public PlanDto(Plan source)
        {
            Id = source.Id;
            FrequencyDto = source.Frequency is null ? null : new FrequencyDto(source.Frequency);
            ValidityDto = source.Validity is null ? null : new ValidityDto(source.Validity);
            PurchaseDay = source.PurchaseDay;
        }

        public string Id { get; set; }

        public FrequencyDto FrequencyDto { get; set; }

        public ValidityDto ValidityDto { get; set; }

        public string PurchaseDay { get; set; }
    }

    public class FrequencyDto
    {
        public FrequencyDto()
        {
        }

        public FrequencyDto(Frequency source)
        {
            Periodicity = source.Periodicity;
            Interval = source.Interval;
        }

        public string Periodicity { get; set; }

        public int Interval { get; set; }
    }

    public class ValidityDto
    {
        public ValidityDto()
        {
        }

        public ValidityDto(Validity source)
        {
            Begin = source.Begin;
            End = source.End;
        }

        public string Begin { get; set; }

        public string End { get; set; }
    }

    public class ShippingAddressDto
    {
        public ShippingAddressDto()
        {
        }

        public ShippingAddressDto(ShippingAddress source)
        {
            AddressId = source.AddressId;
            AddressType = source.AddressType;
        }

        public string AddressId { get; set; }

        public string AddressType { get; set; }
    }

    public class PurchaseSettingsDto
    {
        public PurchaseSettingsDto()
        {
        }

        public PurchaseSettingsDto(PurchaseSettings source)
        {
            PaymentMethodDto = source.PaymentMethod is null ? null : new PaymentMethodDto(source.PaymentMethod);
            CurrencyCode = source.CurrencyCode;
            SelectedSla = source.SelectedSla;
            SalesChannel = source.SalesChannel;
            Seller = source.Seller;
        }

        public PaymentMethodDto PaymentMethodDto { get; set; }

        public string CurrencyCode { get; set; }

        public string? SelectedSla { get; set; }

        public string SalesChannel { get; set; }

        public string Seller { get; set; }
    }

    public class PaymentMethodDto
    {
        public PaymentMethodDto()
        {
        }

        public PaymentMethodDto(PaymentMethod source)
        {
            PaymentAccountId = source.PaymentAccountId;
            PaymentSystem = source.PaymentSystem;
            Installments = source.Installments;
            PaymentSystemName = source.PaymentSystemName;
            PaymentSystemGroup = source.PaymentSystemGroup;
        }

        public string? PaymentAccountId { get; set; }

        public string PaymentSystem { get; set; }

        public int Installments { get; set; }

        public string PaymentSystemName { get; set; }

        public string PaymentSystemGroup { get; set; }
    }

    public class ItemDto
    {
        public ItemDto()
        {
        }

        public ItemDto(Item source)
        {
            Id = source.Id;
            SkuId = source.SkuId;
            Quantity = source.Quantity;
            IsSkipped = source.IsSkipped;
            Status = source.Status;
            CycleCount = source.CycleCount;
            PriceAtSubscriptionDate = source.PriceAtSubscriptionDate;
            ManualPrice = source.ManualPrice;
            AttachmentDtos = source.Attachments?.Select(attachment => new AttachmentDto(attachment)).ToList();
        }

        public string Id { get; set; }

        public string SkuId { get; set; }

        public int Quantity { get; set; }

        public bool IsSkipped { get; set; }

        public string Status { get; set; }

        public int CycleCount { get; set; }

        public decimal PriceAtSubscriptionDate { get; set; }

        public decimal ManualPrice { get; set; } 

        public List<AttachmentDto> AttachmentDtos { get; set; }
    }

    public class AttachmentDto
    {
        public AttachmentDto()
        {
        }

        public AttachmentDto(Attachment source)
        {
            Name = source.Name;
            Content = source.Content is null ? null : new AttachmentContentDto(source.Content);
        }

        public string Name { get; set; }

        public AttachmentContentDto Content { get; set; }
    }

    public class AttachmentContentDto
    {
        public AttachmentContentDto()
        {
        }

        public AttachmentContentDto(AttachmentContent source)
        {
            AdditionalData = source.AdditionalData;
        }

        public Dictionary<string, object> AdditionalData { get; set; }
    }

    public class CustomDataDto
    {
        public CustomDataDto()
        {
        }

        public CustomDataDto(CustomData source)
        {
            AdditionalData = source.AdditionalData;
        }

        public Dictionary<string, object> AdditionalData { get; set; }
    }
}
