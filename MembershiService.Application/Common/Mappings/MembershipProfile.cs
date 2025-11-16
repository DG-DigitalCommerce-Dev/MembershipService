using AutoMapper;
using MembershipService.Application.Common.Models;
using MembershipService.Application.DTOs;
using MembershipService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MembershipService.Application.Common.Mappings
{
    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {
            // Map Between MembershipResponse and MembershipDto
            CreateMap<MembershipDto, MembershipResponse>()
                .ForMember(dest => dest.Plan, opt => opt.MapFrom(src => src.PlanDto))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddressDto))
                .ForMember(dest => dest.PurchaseSettings, opt => opt.MapFrom(src => src.PurchaseSettingsDto))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ItemDtos));

            CreateMap<PlanDto, Plan>()
                .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.FrequencyDto))
                .ForMember(dest => dest.Validity, opt => opt.MapFrom(src => src.ValidityDto));

            CreateMap<PurchaseSettingsDto, PurchaseSettings>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethodDto));

            CreateMap<ItemDto, Item>()
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.AttachmentDtos));

            CreateMap<FrequencyDto, Frequency>();
            CreateMap<ValidityDto, Validity>();
            CreateMap<ShippingAddressDto, ShippingAddress>();
            CreateMap<PaymentMethodDto, PaymentMethod>();
            CreateMap<AttachmentDto, Attachment>();
            CreateMap<AttachmentContentDto, AttachmentContent>();
            CreateMap<CustomDataDto, CustomData>();

            // Map Between MembershipData and MembershipDto
            CreateMap<MembershipData, MembershipDto>()
                .ForMember(dest => dest.PlanDto, opt => opt.MapFrom(src => src.Plan))
                .ForMember(dest => dest.ShippingAddressDto, opt => opt.MapFrom(src => src.ShippingAddress))
                .ForMember(dest => dest.PurchaseSettingsDto, opt => opt.MapFrom(src => src.PurchaseSettings))
                .ForMember(dest => dest.ItemDtos, opt => opt.MapFrom(src => src.Items));

            CreateMap<PlanData, PlanDto>()
                .ForMember(dest => dest.FrequencyDto, opt => opt.MapFrom(src => src.Frequency))
                .ForMember(dest => dest.ValidityDto, opt => opt.MapFrom(src => src.Validity));

            CreateMap<PurchaseSettingsData, PurchaseSettingsDto>()
                .ForMember(dest => dest.PaymentMethodDto, opt => opt.MapFrom(src => src.PaymentMethod));

            CreateMap<ItemData, ItemDto>()
                .ForMember(dest => dest.AttachmentDtos, opt => opt.MapFrom(src => src.Attachments));

            CreateMap<FrequencyData, FrequencyDto>();
            CreateMap<ValidityData, ValidityDto>();
            CreateMap<ShippingAddressData, ShippingAddressDto>();
            CreateMap<PaymentMethodData, PaymentMethodDto>();
            CreateMap<AttachmentData, AttachmentDto>();
            CreateMap<AttachmentContentData, AttachmentContentDto>();
            CreateMap<CustomDataObject, CustomDataDto>();
        }
    }
}

