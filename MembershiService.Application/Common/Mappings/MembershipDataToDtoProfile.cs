using AutoMapper;
using MembershipService.Application.DTOs;
using MembershipService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MembershipService.Application.Common.Mappings
{
    public class MembershipDataToDtoProfile : Profile
    {
        public MembershipDataToDtoProfile()
        {
            CreateMap<MembershipData, MembershipDtoData>()
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

